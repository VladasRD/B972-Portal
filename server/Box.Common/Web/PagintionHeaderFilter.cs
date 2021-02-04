using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Box.Common.Web
{
    public class PaginationHeaderFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if(!context.HttpContext.Items.ContainsKey("TotalCount"))
            {
                return;
            }
            int count = (int) context.HttpContext.Items["TotalCount"];            
            context.HttpContext.Response.Headers.Add("access-control-expose-headers", "x-total-count");
            context.HttpContext.Response.Headers.Add("x-total-count", count.ToString());
        }        
        
    }

    public static class PaginationHeaderExtension
    {
        public static void SetListTotalCount(this HttpRequest request, int count)
        {
            request.HttpContext.Items["TotalCount"] = count;
        }
    }

    public class OptionalOutTotalCount
    {
        public int Value { get; set; }
    }
}
