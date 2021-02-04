using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box.CMS.Services;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Box.CMS.Models;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Box.CMS.Api {

    [Route("api/[controller]")]
    public class CMSFilesController : Controller {

        private readonly CMSService _cmsService;

        private readonly Box.Security.Services.LogService log;

        private readonly IStringLocalizer<Box.Common.Strings> Strings;

        public CMSFilesController(
            CMSService cmsService,
            Box.Security.Services.LogService logService,
            IStringLocalizer<Box.Common.Strings> strings,
            IHostingEnvironment env) {
            this._cmsService = cmsService;
            this.log = logService;
            this.Strings = strings;
        }

        [HttpGet]          
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IEnumerable<File> Get([FromQuery] string folder, [FromQuery] string filter = null, [FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] bool unUsed = false) {
            
            _cmsService.VerifyAuthorizationToEditFiles(User);
            
            var totalCount = new OptionalOutTotalCount();

            var files = _cmsService.GetFiles(filter, skip, top, folder, unUsed, totalCount).ToList();
            
            Request.SetListTotalCount(totalCount.Value);

            return files;
        }

        [HttpDelete("{folder}/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles="CMS_FILE.DELETE")]
        public void Delete([FromRoute] string folder, [FromRoute] string id, [FromQuery] bool unUsed = false) {
            
            File file;
            
            if (unUsed) {
                _cmsService.RemoveUnusedFiles();
                log.Log(Strings["UNUSED_FILES_REMOVED"]);
            } else {
                file = _cmsService.GetFile(id);
                if (folder.ToLower() != file.Folder.ToLower())
                    throw new System.Security.SecurityException("Could not delete file - wrong folder");                
                _cmsService.RemoveFile(id);                
            }
    
        }

        [HttpPost("{folder}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles="CMS_FILE.UPLOAD")]
        public async Task<Box.CMS.Models.File[]> Post(List<IFormFile> files, [FromRoute] string folder, [FromQuery] int storage = 0) {

            List<Box.CMS.Models.File> boxFiles = new List<Box.CMS.Models.File>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {

                    byte[] bytes;                    
                    using (var memoryStream = new System.IO.MemoryStream()) {
                        await formFile.CopyToAsync(memoryStream);
                        bytes = memoryStream.ToArray();
                    }

                    Box.CMS.Models.File boxFile = new Box.CMS.Models.File();
                    boxFile.FileUId = Guid.NewGuid().ToString();
                    boxFile.FileName = _cmsService.CleanFileName(formFile.FileName);
                    boxFile.Type = formFile.ContentType;
                    boxFile.Folder = (folder == null ? "Images" : folder);
                    boxFile.Size = bytes.Length;
                    boxFile.Data = new FileData() { FileUId = boxFile.FileUId, StoredData = bytes };

                    _cmsService.SetFileThumb(boxFile);

                    _cmsService.SaveFile(boxFile, (FileStorages)storage);

                    boxFile.Data = null;
                    boxFiles.Add(boxFile);
                }
            }

            return boxFiles.ToArray();

        }

    }
}
