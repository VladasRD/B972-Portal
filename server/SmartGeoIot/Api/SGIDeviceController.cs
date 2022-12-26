using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using SmartGeoIot.Models;

namespace SmartGeoIot.Api
{
    [Route("api/[controller]")]
    public class SGIDeviceController : Controller
    {
        protected readonly SmartGeoIot.Services.RadiodadosService _sgiService;
        public SGIDeviceController(SmartGeoIot.Services.RadiodadosService sgiService)
        {
            _sgiService = sgiService;
        }

        [HttpGet]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.READ")]
        public IEnumerable<Device> Get([FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string filter = null, [FromQuery] string scope = null)
        {
            var totalCount = new OptionalOutTotalCount();
            
            bool isFullAcess = false;
            if (User.IsInRole("SGI.MASTER"))
                isFullAcess = true;

            // passar o "User", para ver se tem acesso aos devices do cliente
            var devices = _sgiService.GetDevices(User, skip, top, filter, totalCount, isFullAcess, scope);
            Request.SetListTotalCount(totalCount.Value);
            return devices;
        }

        [HttpGet("fromDashboard")]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.READ")]
        public IEnumerable<DeviceRegistration> FromDashboard([FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string filter = null)
        {
            var totalCount = new OptionalOutTotalCount();
            
            bool isFullAcess = false;
            if (User.IsInRole("SGI.MASTER"))
                isFullAcess = true;

            // passar o "User", para ver se tem acesso aos devices do cliente
            var devices = _sgiService.GetDevicesFromDashboard(User, skip, top, filter, totalCount, isFullAcess);
            Request.SetListTotalCount(totalCount.Value);
            return devices;
        }

        [HttpGet("fromFirmware")]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI.MASTER")]
        public IEnumerable<DeviceRegistration> fromFirmware([FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string filter = null, [FromQuery] string client = null)
        {
            var totalCount = new OptionalOutTotalCount();
            
            if (!User.IsInRole("SGI.MASTER"))
                throw new Box.Common.BoxLogicException("Você não tem permissão de acessar esta funcionalidade.");

            // passar o "User", para ver se tem acesso aos devices do cliente
            var devices = _sgiService.GetDevicesWithDataFirmware(User, skip, top, filter, client, totalCount);
            Request.SetListTotalCount(totalCount.Value);
            return devices;
        }

        [HttpGet("ofClient/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.READ")]
        public IEnumerable<DeviceRegistration> ofClient(string id)
        {
            var totalCount = new OptionalOutTotalCount();
            
            bool isFullAcess = false;
            if (User.IsInRole("SGI.MASTER"))
                isFullAcess = true;

            // passar o "User", para ver se tem acesso aos devices do cliente
            var devices = _sgiService.GetDevicesOfClient(User, isFullAcess, id);
            Request.SetListTotalCount(totalCount.Value);
            return devices;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.READ")]
        public Device Get(string id)
        {
            if(id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            // verificar se user tem acesso a esse dispositivo
            // passar o "User", para ver se tem acesso aos devices do cliente

            return _sgiService.GetDevice(id);
        }
    }
}