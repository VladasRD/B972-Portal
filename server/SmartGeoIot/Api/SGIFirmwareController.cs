using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using SmartGeoIot.Models;
using SmartGeoIot.ViewModels;

namespace SmartGeoIot.Api
{
    [Route("api/[controller]")]
    public class SGIFirmwareController : Controller
    {
        protected readonly SmartGeoIot.Services.RadiodadosService _sgiService;
        public SGIFirmwareController(SmartGeoIot.Services.RadiodadosService sgiService)
        {
            _sgiService = sgiService;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI.MASTER")]
        public FirmwareViewModels Get(string id)
        {
            if(id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            if (!User.IsInRole("SGI.MASTER"))
                throw new Box.Common.BoxLogicException("Você não tem permissão de acessar os dados de firmware.");

            return _sgiService.GetFirmware(id);
        }
        
    }
}