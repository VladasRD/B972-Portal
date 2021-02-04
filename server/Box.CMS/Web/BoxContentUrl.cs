using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Box.CMS.Web
{

public class BoxContentUrl
    {
        private readonly RequestDelegate _next;

        private readonly ConcurrentDictionary<string, string> redirectCache = new ConcurrentDictionary<string, string>();

        public BoxContentUrl(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            
            SetRedirectPath(context);
            await _next.Invoke(context);

            // Clean up.
        }

        private void SetRedirectPath(HttpContext context) {

            //string fullUrl = BoxLib.RemoveAppNameFromUrl(context.Request.Path);
            string url = context.Request.Path.ToUriComponent();;
            string redirectUrl = null;
            bool useThumbRedirect = false;

            if (url.StartsWith("/where-is-my-db.htm") || url.StartsWith("/files/") || url.StartsWith("/box_templates/"))
                return;

            // if true, can see not published contents
            bool canSeeOnlyPublished = GetShowOnlyPublished(context, url);
            
            if (!context.Request.QueryString.HasValue) {
                redirectCache.TryGetValue(url, out redirectUrl);
            }

            if (redirectUrl == null) {

                Services.CMSService cms = context.RequestServices.GetService(typeof(Box.CMS.Services.CMSService)) as Services.CMSService;

                Models.ContentHead c = null;
                try
                {
                    c = cms.GetContentHeadByUrlAndKind(url, null, canSeeOnlyPublished);                    
                }                
                catch (System.Exception ex)
                {
                    // var SQLexception = Box.Core.Services.SecurityService.GetSqlException(ex);
                    // if (SQLexception != null && !Box.Core.Services.SecurityService.IsDebug)
                    //     app.Context.Response.Redirect("~/where-is-my-db.htm#" + SQLexception.Number);
                    // else
                    //     throw ex;
                    throw ex;
                }

                if (c == null)
                    return;

                useThumbRedirect = SetThumbRedirectUrl(context, c);
                
                if(!useThumbRedirect) {
                    redirectUrl = "/box_templates/" + c.Kind + "/" + c.ContentUId;
                    // only add at cache published urls
                    if(canSeeOnlyPublished)
                        redirectCache.TryAdd(url, redirectUrl);
                }                
                
            }

            if(!useThumbRedirect) {
                context.Request.Path = redirectUrl;
                // context.Request.HttpContext.Items["_boxOriginalUrl"] = url;
            }

        }

        private bool SetThumbRedirectUrl(HttpContext context, Models.ContentHead head) {
            var imgUrl = head.ThumbFilePath.Replace("?asThumb=true", "");
            if (context.Request.Query.ContainsKey("asImg")) {
                context.Request.Path = imgUrl;
                return true;
            }
            if (!context.Request.Query.ContainsKey("asThumb")) {
                return false;
            }
            context.Request.Path = imgUrl;
            context.Request.QueryString = new QueryString("?asThumb=true");
            return true;    
        }

        private bool GetShowOnlyPublished(HttpContext context, string url)
        {
          return false;
          
            // if (!context.Request.Query.ContainsKey("previewToken"))
            //     return true;
            
            // //Remove token from url
            // string token = context.Request.Query["previewToken"].ToString();

            // Core.Services.SecurityService security = new Core.Services.SecurityService();
            
            // User u = security.GetUserByAuthToken(token);
            // if (u == null)
            //     return true;
            
            // string[] roles = security.GetUserRoles(u);
            // return !roles.Contains(ADM_CMS_group.UserGroupUId) && !roles.Contains(AMD_SEC_group.UserGroupUId);                        
        }

    }

    public static class BoxContentUrlExtensions
    {
        public static IApplicationBuilder UseBoxContentUrls(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BoxContentUrl>();
        }
    }
}