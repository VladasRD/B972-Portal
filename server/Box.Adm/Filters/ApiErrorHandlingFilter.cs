using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Box.Adm.Filters
{
    public class ApiErrorHandlingFilter : ExceptionFilterAttribute
    {

        private Box.Security.Services.LogService _log { get; set; }

        public ApiErrorHandlingFilter(Box.Security.Services.LogService log) {
            this._log = log;
        }


        public override void OnException(ExceptionContext context)
        {
            object response = new { message = "Unknow error" };

            if (context.Exception is Box.Common.BoxLogicException)
            {
                var bEx = context.Exception as Box.Common.BoxLogicException;
                response = new { message = context.Exception.Message };                
                context.HttpContext.Response.StatusCode = (int) bEx.HttpCode;

                _log.Log(context.Exception.Message, String.IsNullOrEmpty(bEx.Details) ? "Box Logic Exception" : bEx.Details);
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                response = new { message = "UnauthorizedAccessException" };
                context.HttpContext.Response.StatusCode = 401;     

                _log.Log("Unauthorized Access", "Unauthorized Access");
            }
            else
            {
                // Unhandled errors
                #if !DEBUG
                var msg = "An unhandled error occurred.";                
                string stack = null;
                #else
                var msg = context.Exception.GetBaseException().Message;
                string stack = context.Exception.StackTrace;
                #endif
                
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new { message = msg, stackTrace = stack };

                _log.Log("Internal Error", context.Exception.GetBaseException().Message, true);
            }

            // always return a JSON result
            context.Result = new JsonResult(response);

            base.OnException(context);
        }
    }
}
