using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using SmartGeoIot.ViewModels;
using SmartGeoIot.Extensions;
using System.Linq;

namespace SmartGeoIot.Api
{
    [Route("api/[controller]")]
    public class SGIDashboardController : Controller
    {
        protected readonly SmartGeoIot.Services.SmartGeoIotService _sgiService;
        public SGIDashboardController(SmartGeoIot.Services.SmartGeoIotService sgiService)
        {
            _sgiService = sgiService;
        }

        [HttpGet]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DASHBOARD.READ")]
        public IEnumerable<DashboardViewModels> Get([FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string filter = null)
        {
            var totalCount = new OptionalOutTotalCount();

            bool isFullAcess = false;
            if (User.IsInRole("SGI.MASTER"))
                isFullAcess = true;

            var dashboard = _sgiService.GetDashboards(User, totalCount, isFullAcess);
            Request.SetListTotalCount(totalCount.Value);

            return dashboard;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DASHBOARD.READ")]
        public DashboardViewModels Get(string id)
        {
            if (id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            var dashboard = _sgiService.GetDashboard(id);
            if (!User.IsInRole("SGI.MASTER"))
            {
                var userDevices = _sgiService.GetUserDevices(User.GetId());
                if (!userDevices.Any(c => c.Id == id))
                    throw new Box.Common.BoxLogicException("Usuário não tem permissão para acessar dados deste dispositivo.");
            }

            return dashboard;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DASHBOARD.READ")]
        public void Put([FromQuery] string id, [FromQuery] int numeroEnvios, [FromQuery] int tempoTransmissao, [FromQuery] bool tipoEnvio, [FromQuery] int tensaoMinima)
        {
            var device = _sgiService.GetDevice(id);
            this._sgiService.SigfoxSendChangesDeviceTypes(device, numeroEnvios, tempoTransmissao, tipoEnvio, tensaoMinima);
        }
        
    }
}