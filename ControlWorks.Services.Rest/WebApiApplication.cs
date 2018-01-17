using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ControlWorks.Services.Bartender;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Configuration;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Owin;

namespace ControlWorks.Services.Rest
{
    public class WebApiApplication
    {
        private ILog _log = LogManager.GetLogger("FileLogger");

        private static string _port = "9001";

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            config.MapHttpAttributeRoutes();

            // Configure Web API for self-host. 
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{width}/{height}/{*filename}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "PrintApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ActionRoute",
                routeTemplate: "api/{controller}/{action}/{item}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
        }

        public static void Start()
        {

            var hostUrl = $"http://*:{_port}";
            //Logger.Log(new LogEntry(LoggingEventType.Information, $"Starting WebApi at host {hostUrl}"));
            WebApp.Start<WebApiApplication>(hostUrl);

        }

        public static void Stop()
        {
            BartenderService.Service.Dispose();
        }
    }
}
