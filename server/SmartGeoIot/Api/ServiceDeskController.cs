using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using SmartGeoIot.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace SmartGeoIot.Api
{
    [Route("api/[controller]")]
    public class ServiceDeskController : Controller
    {
        protected readonly SmartGeoIot.Services.RadiodadosService _sgiService;
        public ServiceDeskController(SmartGeoIot.Services.RadiodadosService sgiService)
        {
            _sgiService = sgiService;
        }

        [HttpGet]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DASHBOARD.READ")]
        public IEnumerable<Models.ServiceDesk> Get([FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string filter = null)
        {
            var totalCount = new OptionalOutTotalCount();

            var serviceDesks = _sgiService.GetServiceDesks(skip, top, filter, totalCount);
            Request.SetListTotalCount(totalCount.Value);

            return serviceDesks;
        }

        [HttpGet("{deviceId}")]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DASHBOARD.READ")]
        public IEnumerable<Models.ServiceDeskRecord> GetHistory(string deviceId, [FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string filter = null)
        {
            var totalCount = new OptionalOutTotalCount();

            if (deviceId == null)
                throw new Box.Common.BoxLogicException("É necessário informar um dispositivo.");

            var serviceDesk = _sgiService.GetServiceDeskHistory(deviceId, skip, top, filter, totalCount);
            Request.SetListTotalCount(totalCount.Value);
            return serviceDesk;
        }

        // [HttpPost]
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DASHBOARD.WRITE")]
        // public Models.ServiceDesk Post([FromBody] Models.ServiceDesk serviceDesk)
        // {
        //     if (serviceDesk == null)
        //         throw new Box.Common.BoxLogicException("Chamado inválido.");
                
        //     return this._sgiService.SaveServiceDesk(serviceDesk);
        // }

        // [HttpPut("{id}")]
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DASHBOARD.WRITE")]
        // public Models.ServiceDesk Put(int id, [FromBody] Models.ServiceDesk serviceDesk)
        // {
        //     if (serviceDesk == null)
        //         throw new Box.Common.BoxLogicException("Chamado inválido.");

        //     if (id != serviceDesk.ServiceDeskId)
        //         throw new Box.Common.BoxLogicException("Id inválido.");

        //     return this._sgiService.SaveServiceDesk(serviceDesk);
        // }

        [HttpPut("by-dashboard/{deviceId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SERVICE-DESK.WRITE")]
        public void Put(string deviceId, [FromQuery] string reason, [FromQuery] string package = null, [FromQuery] long? packageTimestamp = null)
        {
            this._sgiService.SaveServiceDeskByDashboard(deviceId, reason, package, packageTimestamp);
        }

        [HttpPut("close/{deviceId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SERVICE-DESK.WRITE")]
        public void PutClose(string deviceId, [FromQuery] string reason)
        {
            this._sgiService.CloseServiceDesk(deviceId, reason);
        }


    }
}