using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using System;

namespace SmartGeoIot.Api
{
    [Route("api/[controller]")]
    public class SGIDeviceRegistrationController : Controller
    {
        protected readonly SmartGeoIot.Services.RadiodadosService _sgiService;
        public SGIDeviceRegistrationController(SmartGeoIot.Services.RadiodadosService sgiService)
        {
            _sgiService = sgiService;
        }

        [HttpGet]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.READ")]
        public IEnumerable<Models.DeviceRegistration> Get([FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string filter = null)
        {
            var totalCount = new OptionalOutTotalCount();

            var deviceRegistration = _sgiService.GetDevicesRegistrations(skip, top, filter, totalCount);
            Request.SetListTotalCount(totalCount.Value);

            return deviceRegistration;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.READ")]
        public Models.DeviceRegistration Get(string id)
        {
            if (id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            var deviceRegistration = _sgiService.GetDeviceRegistration(id);
            return deviceRegistration;
        }

        [HttpGet("byDeviceId/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.READ")]
        public Models.DeviceRegistration GetByDeviceId(string id)
        {
            if (id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            var deviceRegistration = _sgiService.GetDeviceRegistrationById(id);
            return deviceRegistration;
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.WRITE")]
        public void Delete(string id)
        {
            _sgiService.DeleteDeviceRegistration(id);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.WRITE")]
        public Models.DeviceRegistration Post([FromBody] Models.DeviceRegistration deviceRegistration)
        {
            if (deviceRegistration == null)
                throw new Box.Common.BoxLogicException("Dispositivo inválido.");
            
            if (!this._sgiService.CanRegisterDevice(deviceRegistration.DeviceId))
                throw new Box.Common.BoxLogicException("Dispositivo já cadastrado.");
                
            deviceRegistration.DeviceCustomUId = Guid.NewGuid().ToString();
            return this._sgiService.SaveDeviceRegistration(deviceRegistration);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.WRITE")]
        public Models.DeviceRegistration Put(string id, [FromBody] Models.DeviceRegistration deviceRegistration)
        {
            if (deviceRegistration == null)
                throw new Box.Common.BoxLogicException("Dispositivo inválido.");

            if (id != deviceRegistration.DeviceCustomUId)
                throw new Box.Common.BoxLogicException("Id inválido.");

            return this._sgiService.SaveDeviceRegistration(deviceRegistration);
        }

        [HttpPut("change-serial-number/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.READ")]
        public Models.DeviceRegistration PutChangeSerialNumber(string id, [FromQuery] string serialNumber)
        {
            if (id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            return _sgiService.ChangeSerialNumberByDevide(id, serialNumber);
        }

        [HttpPut("change-model/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.READ")]
        public Models.DeviceRegistration PutChangeModel(string id, [FromQuery] string model)
        {
            if (id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            return _sgiService.ChangeModelByDevide(id, model);
        }

        [HttpPut("change-notes/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.READ")]
        public Models.DeviceRegistration PutChangeNotes(string id, [FromQuery] string notes)
        {
            if (id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            return _sgiService.ChangeNotesByDevide(id, notes);
        }

        [HttpPut("change-fields-trm11/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-DEVICE.READ")]
        public Models.DeviceRegistration PutChangeFieldsTRM11(string id, [FromQuery] string field, [FromQuery] string value)
        {
            if (id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            return _sgiService.ChangeFieldByDevide(id, field, value);
        }
        
    }
}