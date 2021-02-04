using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using System;

namespace SmartGeoIot.Api
{
    [Route("api/[controller]")]
    public class SGIOutgoingController : Controller
    {
        protected readonly SmartGeoIot.Services.SmartGeoIotService _sgiService;
        public SGIOutgoingController(SmartGeoIot.Services.SmartGeoIotService sgiService)
        {
            _sgiService = sgiService;
        }

        [HttpGet]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI.MASTER-COMMERCIAL")]
        public IEnumerable<Models.Outgoing> Get([FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string filter = null, [FromQuery] int month = 0, [FromQuery] int year = 0)
        {
            var totalCount = new OptionalOutTotalCount();

            var outgoings = _sgiService.GetOutgoings(skip, top, filter, month, year, totalCount);
            Request.SetListTotalCount(totalCount.Value);

            return outgoings;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI.MASTER-COMMERCIAL")]
        public Models.Outgoing Get(string id)
        {
            if (id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id.");

            var outgoing = _sgiService.GetOutgoing(id);
            return outgoing;
        }

        [HttpGet("show/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI.MASTER-COMMERCIAL")]
        public Models.OutgoingViewModel show(string id)
        {
            if (id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id.");

            var outgoing = _sgiService.GetOutgoingShow(id);
            return outgoing;
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI.MASTER-COMMERCIAL")]
        public void Delete(string id)
        {
            _sgiService.DeleteOutgoing(id);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI.MASTER-COMMERCIAL")]
        public Models.Outgoing Post([FromBody] Models.Outgoing outgoing)
        {
            if (outgoing == null)
                throw new Box.Common.BoxLogicException("Outgoing inválido.");
                
            outgoing.OutgoingUId = Guid.NewGuid().ToString();
            return this._sgiService.SaveOutgoing(outgoing);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI.MASTER-COMMERCIAL")]
        public Models.Outgoing Put(string id, [FromBody] Models.Outgoing outgoing)
        {
            if (outgoing == null)
                throw new Box.Common.BoxLogicException("Outgoing inválido.");

            if (id != outgoing.OutgoingUId)
                throw new Box.Common.BoxLogicException("Id inválido.");

            return this._sgiService.SaveOutgoing(outgoing);
        }
        
    }
}