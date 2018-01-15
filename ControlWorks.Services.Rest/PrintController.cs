using System;
using System.Collections.Generic;
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
                string previewFile;
                using (var service = new BartenderService())
                {
                    previewFile = service.GetPreviewImage(filename, width, height);
                }

                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, previewFile));
            }
            catch (Exception ex)
            {
                ex.Data.Add("PviController.Operation", "GetDetails");
                _log.Error(ex.Message, ex);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }

        }

        [HttpPost]
        public IHttpActionResult SendPrint([FromBody] string filename)
        {
            try
            {
                string message;
                using (var service = new BartenderService())
                {
                    message = service.Print(filename);
                }

                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, message));
            }
            catch (Exception ex)
            {
                ex.Data.Add("PviController.Operation", "GetDetails");
                _log.Error(ex.Message, ex);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
    }
}
