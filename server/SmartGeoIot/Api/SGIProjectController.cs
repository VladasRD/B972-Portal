using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using System;

namespace SmartGeoIot.Api
{
    [Route("api/[controller]")]
    public class SGIProjectController : Controller
    {
        protected readonly SmartGeoIot.Services.RadiodadosService _sgiService;
        public SGIProjectController(SmartGeoIot.Services.RadiodadosService sgiService)
        {
            _sgiService = sgiService;
        }

        [HttpGet]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-PROJECT.READ")]
        public IEnumerable<Models.Project> Get([FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string filter = null)
        {
            var totalCount = new OptionalOutTotalCount();

            var projects = _sgiService.GetProjects(skip, top, filter, totalCount);
            Request.SetListTotalCount(totalCount.Value);

            return projects;
        }

        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IEnumerable<Models.Project> Get()
        {
            bool isFullAcess = false;
            if (User.IsInRole("SGI.MASTER"))
                isFullAcess = true;

            return _sgiService.GetMeProjects(User, isFullAcess);
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-PROJECT.READ")]
        public Models.Project Get(string id)
        {
            if (id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do projeto.");

            var project = _sgiService.GetProject(id);
            return project;
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-PROJECT.WRITE")]
        public void Delete(string id)
        {
            _sgiService.DeleteProject(id);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-PROJECT.WRITE")]
        public Models.Project Post([FromBody] Models.Project project)
        {
            if (project == null)
                throw new Box.Common.BoxLogicException("Projeto inválido.");
                
            project.ProjectUId = Guid.NewGuid().ToString();
            return this._sgiService.SaveProject(project);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-PROJECT.WRITE")]
        public Models.Project Put(string id, [FromBody] Models.Project project)
        {
            if (project == null)
                throw new Box.Common.BoxLogicException("Projeto inválido.");

            if (id != project.ProjectUId)
                throw new Box.Common.BoxLogicException("Id inválido.");

            return this._sgiService.SaveProject(project);
        }
        
    }
}