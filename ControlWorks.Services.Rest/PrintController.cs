using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ControlWorks.Services.Bartender;
using log4net;

namespace ControlWorks.Services.Rest
{
    public class PrintController : ApiController
    {
        private readonly ILog _log = LogManager.GetLogger("FileLogger");

        [HttpGet]
        public IHttpActionResult GetPreview(int width, int height, string filename) 
        {
            //http://localhost:9001/api/Print/GetPreview
            try
            {
                var service = BartenderService.Service;
                var previewFile = service.GetPreviewImage(filename, width, height);

                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, previewFile));
            }
            catch (Exception ex)
            {
                ex.Data.Add("PrintController.Operation", "GetPreview");
                _log.Error(ex.Message, ex);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }

        }

        [HttpPut]
        public IHttpActionResult Cancel()
        {
            try
            {
                var service = BartenderService.Service;
                service.Cancel();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, String.Empty));
            }
            catch (Exception ex)
            {
                ex.Data.Add("PrintController.Operation", "Cancel");
                _log.Error(ex.Message, ex);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        public IHttpActionResult Cancel(PrintData item)
        {
            try
            {
                var service = BartenderService.Service;
                service.Cancel(item.PrintCommand);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, String.Empty));
            }
            catch (Exception ex)
            {
                ex.Data.Add("PrintController.Operation", "Cancel");
                _log.Error(ex.Message, ex);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        public IHttpActionResult SendPrint(PrintData item)
        {
            try
            {
                var service = BartenderService.Service;
                var message = service.Print(item);

                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, message));
            }
            catch (Exception ex)
            {
                ex.Data.Add("PrintController.Operation", "SendPrint");
                _log.Error(ex.Message, ex);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpGet]
        public IHttpActionResult Start()
        {
            try
            {
                var service = BartenderService.Service;
                service.Start();

                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Bartender Service Started"));
            }
            catch (Exception ex)
            {
                ex.Data.Add("PrintController.Operation", "Start");
                _log.Error(ex.Message, ex);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpGet]
        public IHttpActionResult Stop()
        {
            try
            {
                var service = BartenderService.Service;
                service.Start();

                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Bartender Service Started"));
            }
            catch (Exception ex)
            {
                ex.Data.Add("PrintController.Operation", "Stop");
                _log.Error(ex.Message, ex);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

    }
}
