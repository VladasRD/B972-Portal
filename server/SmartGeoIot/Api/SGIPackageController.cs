using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using System;

namespace SmartGeoIot.Api
{
    [Route("api/[controller]")]
    public class SGIPackageController : Controller
    {
        protected readonly SmartGeoIot.Services.SmartGeoIotService _sgiService;
        public SGIPackageController(SmartGeoIot.Services.SmartGeoIotService sgiService)
        {
            _sgiService = sgiService;
        }

        [HttpGet]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-PACKAGE.READ")]
        public IEnumerable<Models.Package> Get([FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string filter = null)
        {
            var totalCount = new OptionalOutTotalCount();

            var packages = _sgiService.GetPackages(skip, top, filter, totalCount);
            Request.SetListTotalCount(totalCount.Value);

            return packages;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-PACKAGE.READ")]
        public Models.Package Get(string id)
        {
            if (id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do pacote.");

            var package = _sgiService.GetPackage(id);
            return package;
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-PACKAGE.WRITE")]
        public void Delete(string id)
        {
            _sgiService.DeletePackage(id);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-PACKAGE.WRITE")]
        public Models.Package Post([FromBody] Models.Package package)
        {
            if (package == null)
                throw new Box.Common.BoxLogicException("Pacote inválido.");
                
            package.PackageUId = Guid.NewGuid().ToString();
            return this._sgiService.SavePackage(package);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-PACKAGE.WRITE")]
        public Models.Package Put(string id, [FromBody] Models.Package package)
        {
            if (package == null)
                throw new Box.Common.BoxLogicException("Pacote inválido.");

            if (id != package.PackageUId)
                throw new Box.Common.BoxLogicException("Id inválido.");

            return this._sgiService.SavePackage(package);
        }
        
    }
}