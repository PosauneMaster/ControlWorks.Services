using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Seagull.BarTender.Print;

namespace ControlWorks.Services.Bartender
{
    public class BartenderService : IDisposable
    {
        private readonly ILog _log = LogManager.GetLogger("FileLogger");
        private readonly Engine _engine = null; // The BarTender Print Engine.
        private LabelFormatDocument _format = null; // The format that will be exported.

        public string DefaultPrinterName { get; set; }

        public BartenderService()
        {
            _engine = new Engine(true);

            var printers = new Printers();
            DefaultPrinterName = printers.Default.PrinterName;
        }

        public string Print(string filename)
        {
            try
            {
                _format?.Close(SaveOptions.DoNotSaveChanges);
                _format = _engine.Documents.Open(filename);
                _format.PrintSetup.IdenticalCopiesOfLabel = 1;
                _format.PrintSetup.PrinterName = DefaultPrinterName;
                var waitForCompletionTimeout = 10000; // 10 seconds
                var result = _format.Print("Label Print", waitForCompletionTimeout, out var messages);

                string messageString = "\n\nMessages:";
                foreach (Seagull.BarTender.Print.Message message in messages)
                {
                    messageString += "\n\n" + message.Text;
                }

                _log.Info(messageString);

                return messageString;

            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }

            return "An Error Occurred";
        }


        public bool Reprint()
        {
            if (_format == null)
            {
                return false;
            }

            if (_engine?.Documents == null)
            {
                return false;
            }

            if (_engine.Documents.Count < 1)
            {
                return false;
            }
            var waitForCompletionTimeout = 10000; // 10 seconds
            var result = _format.Print("Label Print", waitForCompletionTimeout, out var messages);

            string messageString = "\n\nMessages:";
            foreach (Seagull.BarTender.Print.Message message in messages)
            {
                messageString += "\n\n" + message.Text;
            }

            _log.Info(messageString);

            return result == Result.Success;
        }


        public string GetPreviewImage(string filename, int width, int height)
        {
            _log.Debug($"GetPreviewImage for filename={filename}");

            var previewPath = ConfigurationManager.AppSettings["PreviewPath"];

            try
            {
                var files = Directory.GetFiles(previewPath);
                foreach (string file in files)
                {
                    File.Delete(file);
                }


                _format = _engine.Documents.Open(filename);
                _format.PrintSetup.PrinterName = DefaultPrinterName;
                Messages messages;
                _format.ExportPrintPreviewToFile(previewPath, "PrintPreview%PageNumber%.jpg", ImageType.JPEG, Seagull.BarTender.Print.ColorDepth.ColorDepth24bit, new Resolution(width, height), System.Drawing.Color.White, OverwriteOptions.Overwrite, true, true, out messages);
                files = Directory.GetFiles(previewPath, "*.*");

                return files.Length < 1 ? String.Empty : files[0];
            }
            catch (Exception ex)
            {
                _log.Error($"BartenderService.GetPreviewImage error for filename={filename}", ex);
            }

            return String.Empty;

        }

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _engine?.Dispose();
            }

            _disposed = true;
        }

        ~BartenderService()
        {
            Dispose(false);
        }
    }
}
