using log4net;
using System;
using Topshelf;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace ControlWorks.Services
{

    class Program
    {
        static void Main(string[] args)
        {

            ILog log = LogManager.GetLogger("FileLogger");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            const string name = "ControlWorksPrintService";
            const string description = "Control Works print service";

            log.Info($"Initializing Service {name} - {description}");

            try
            {
                var host = HostFactory.New(configuration =>
                {
                    configuration.Service<Host>(callback =>
                    {
                        callback.ConstructUsing(s => new Host());
                        callback.WhenStarted(service => service.Start());
                        callback.WhenStopped(service => service.Stop());
                    });
                    configuration.SetDisplayName(name);
                    configuration.SetServiceName(name);
                    configuration.SetDescription(description);
                    configuration.RunAsLocalSystem();
                });
                host.Run();
            }
            catch (Exception ex)
            {
                log.Error("ControlWorksFaspacService Service fatal exception.");
                log.Error(ex.Message, ex);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                System.IO.File.WriteAllText($"FatalErrorlog_{DateTime.Now: yyyyMMddHHMMss}.log", ex.ToString());
            }
        }
    }
}
