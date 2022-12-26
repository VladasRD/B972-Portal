using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using System;

namespace SmartGeoIot.Api
{
    [Route("api/[controller]")]
    public class SGIDeviceLocationController : Controller
    {
        protected readonly SmartGeoIot.Services.RadiodadosService _sgiService;
        public SGIDeviceLocationController(SmartGeoIot.Services.RadiodadosService sgiService)
        {
            _sgiService = sgiService;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.READ")]
        public Models.DeviceLocation Get(string id)
        {
            if (id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            return _sgiService.GetDeviceLocationByDeviceId(id);
        }
        
    }
}