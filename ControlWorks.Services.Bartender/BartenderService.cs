﻿using System;
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
        private static BartenderService _service;

        public static BartenderService Service => _service ?? (_service = new BartenderService());

        private readonly ILog _log = LogManager.GetLogger("FileLogger");
        private Engine _engine = null; // The BarTender Print Engine.
        private LabelFormatDocument _format = null; // The format that will be exported.

        public string DefaultPrinterName { get; set; }

        private BartenderService()
        {
            _engine = new Engine();

            var printers = new Printers();
            DefaultPrinterName = printers.Default.PrinterName;
        }

        public void Start()
        {
            if (_engine == null)
            {
                _engine = new Engine();
            }

            if (!_engine.IsAlive)
            {
                _engine.Start();
            }
        }

        public void Stop()
        {
            _engine?.Stop();
        }

        private int GetNumberOfLabels(string count)
        {
            if (Int32.TryParse(count, out var number))
            {
                return number;
            }

            return 1;
        }

        public string Print(PrintData item)
        {
            try
            {
                Start();

                if (_format.FileName != item.Filename)
                {
                    _format?.Close(SaveOptions.DoNotSaveChanges);
                    _format = _engine.Documents.Open(item.Filename);
                }
               
                _format.PrintSetup.IdenticalCopiesOfLabel = GetNumberOfLabels(item.NumberOfLables);
                _format.PrintSetup.PrinterName = DefaultPrinterName;
                _format.PageSetup.Orientation = GetOrientation(item.Orientation);
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

        private Orientation GetOrientation(string orientation)
        {
            if (orientation == "1")
            {
                return Orientation.Landscape;
            }
            if (orientation == "2")
            {
                return Orientation.Portrait;
            }
            if (orientation == "3")
            {
                return Orientation.Landscape180;
            }
            if (orientation == "4")
            {
                return Orientation.Portrait180;
            }
            return Orientation.Landscape;

        }

        public string GetPreviewImage(string filename, int width, int height)
        {
            _log.Debug($"GetPreviewImage for filename={filename}");

            var previewPath = ConfigurationManager.AppSettings["PreviewPath"];

            try
            {
                Start();
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
