using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using SmartGeoIot.Models;
using System.Threading.Tasks;

namespace SmartGeoIot.Api
{
    [Route("api/b979")]
    public class B979Controller : Controller
    {
        protected readonly SmartGeoIot.Services.RadiodadosService _sgiService;
        public B979Controller(SmartGeoIot.Services.RadiodadosService sgiService)
        {
            _sgiService = sgiService;
        }

        [HttpGet("{apikey}")]
        [PaginationHeaderFilter]
        public StandardPagedResponse<IEnumerable<B979ViewModel>> Get(string apikey, [FromQuery] string deviceId, [FromQuery] string initialDate = null, [FromQuery] string finalDate = null, [FromQuery] int skip = 0, [FromQuery] int top = 0)
        {
            StandardPagedResponse<IEnumerable<B979ViewModel>> response = new StandardPagedResponse<IEnumerable<B979ViewModel>>();
            response.DebugCode = "D39D503F";

            if(apikey == null || !_sgiService.ExistAPIClient(apikey))
            {
                response.MessageToUser = "Chave de API inválida ou não informada.";
                return response;
            }
            if (deviceId != null)
            {
                if (!_sgiService.CanAccessDeviceClient(apikey, deviceId))
                {
                    response.MessageToUser = "Dispositivo inválido.";
                    return response;
                }
            }

            response.MessageToUser = "Success";
            return _sgiService.GetAPIListB979(apikey, response, skip, top, deviceId, initialDate, finalDate);
        }

        [HttpPost("{apikey}")]
        public async Task<IActionResult> Post(string apikey, [FromBody] B979RequestToDevice request)
        {
            StandardResponse<bool> response = new StandardResponse<bool>();
            response.DebugCode = "DC18460A";
            response.Data = true;

            if (request == null)
            {
                response.MessageToUser = "Dados inválidos.";
                response.Data = false;
                return BadRequest(response);
            }
            if(apikey == null || !_sgiService.ExistAPIClient(apikey))
            {
                response.MessageToUser = "Chave de API inválida ou não informada.";
                response.Data = false;
                return BadRequest(response);
            }
            if (request.DeviceId == null)
            {
                response.MessageToUser = "Dispositivo inválido ou não informado.";
                response.Data = false;
                return BadRequest(response);
            }
            if (request.DeviceId != null)
            {
                if (!_sgiService.CanAccessDeviceClient(apikey, request.DeviceId))
                {
                    response.MessageToUser = "Dispositivo inválido.";
                    response.Data = false;
                    return BadRequest(response);
                }
            }
            
            await _sgiService.SaveB979RequestToDevice(request);

            response.MessageToUser = "Dados recebidos com sucesso!";
            return Ok(response);
        }

    }
}