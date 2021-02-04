using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Box.CMS.Models;
using Box.CMS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Box.CMS.Controllers {

    public class CMSFilesReadOnlyController : Controller {
        
        protected readonly CMS.Services.CMSService _cmsService;

        private string webPath;

        public CMSFilesReadOnlyController(CMSService cmsService, IHostingEnvironment env) {
            this._cmsService = cmsService;   
            this.webPath = env.WebRootPath;         
        }

        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 2678400, Location = ResponseCacheLocation.Any)]
        public FileResult Index(string folder, string id, bool? asThumb, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, double scale = 1.0, string vAlign = "center", string hAlign = "center", string mode = null) {

            File file = null;

            if (asThumb==true)
                file = _cmsService.GetFileThumb(id);
            else
                file = _cmsService.GetFile(id);

            if (file == null)
                return null;

            if (file.Folder.ToLower() != folder.ToLower())
                throw new System.Security.SecurityException("File folder does not match");

            if (asThumb.HasValue && asThumb.Value) {
                var type = file.Type;
                if(!type.StartsWith("image"))
                    type = "image/png";
                if (file.Data==null) {
                    return GetLoadingImage();
                }
                return new FileContentResult(file.Data.StoredThumbData, type);
            }
                        
            if (width == 0 && height == 0)
                return new FileContentResult(_cmsService.GetScaledImageFile(file.Data.StoredData, scale, mimeType: file.Type), file.Type);

            return new FileContentResult(_cmsService.GetImageFileThumb(file.Data.StoredData, width, height, maxWidth, maxHeight, vAlign, hAlign, file.Type, mode), file.Type);
        }


        protected FileResult GetLoadingImage() {            
            var path = System.IO.Path.Combine(webPath, "images/cms/file-icons/loading.gif");            
            return new PhysicalFileResult (path, "image/gif");
        }
     

    }
}
