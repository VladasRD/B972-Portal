using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Box.Common.Web;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.Extensions;
using SmartGeoIot.Models;
using SmartGeoIot.ViewModels;

namespace SmartGeoIot.Services
{
    public partial class RadiodadosService
    {
        public IEnumerable<DeviceRegistration> GetDevicesRegistrations(int skip = 0, int top = 0, string filter = null, OptionalOutTotalCount totalCount = null)
        {
            IQueryable<DeviceRegistration> devicesRegistrations = _context.DevicesRegistration.Include(i => i.Device).Include(i => i.Package).Include(i => i.Project);

            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                devicesRegistrations = devicesRegistrations.Where(c =>
                    c.Name.ToLower().Contains(filter) ||
                    c.DeviceId.ToLower().Contains(filter) ||
                    c.NickName.ToLower().Contains(filter)
                    );
            }

            // ordernação
            devicesRegistrations = devicesRegistrations.OrderBy(c => c.Name);

            if (totalCount != null)
                totalCount.Value = devicesRegistrations.Count();

            if (skip != 0)
                devicesRegistrations = devicesRegistrations.Skip(skip);

            if (top != 0)
                devicesRegistrations = devicesRegistrations.Take(top);

            return devicesRegistrations.ToArray();
        }

        public DeviceRegistration GetDeviceRegistration(string id)
        {
            return _context.DevicesRegistration.Find(id);
        }

        public DeviceRegistration GetDeviceRegistrationById(string id)
        {
            return _context.DevicesRegistration.SingleOrDefault(c => c.DeviceId == id);
        }

        public DeviceRegistration GetDeviceRegistrationFull(string id)
        {
            return _context.DevicesRegistration.Include(i => i.Project).Include(i => i.Package).SingleOrDefault(r => r.DeviceId == id);
        }

        public bool VerifyDeviceIsProjectB987(string deviceId)
        {
            var deviceFather = GetDeviceFather(deviceId);
            var deviceReg = GetDeviceRegistrationFull(deviceFather);
            if (deviceReg != null && deviceReg.Project.Code.ToLower() == Utils.EnumToAnnotationText(ProjectCode.B987).ToLower())
                return true;

            return false;
        }

        public Models.DeviceRegistration SaveDeviceRegistration(Models.DeviceRegistration deviceRegistration)
        {
            Models.DeviceRegistration oldDeviceRegistration = GetDeviceRegistration(deviceRegistration.DeviceCustomUId);

            if (oldDeviceRegistration == null)
            {
                if (!CanRegisterDevice(deviceRegistration.DeviceId))
                    throw new Box.Common.BoxLogicException("Dispositivo já cadastrado.");
                
                _context.Entry<Models.DeviceRegistration>(deviceRegistration).State = EntityState.Added;
            }
            else
            {
                _context.DevicesRegistration.Attach(oldDeviceRegistration);
                _context.Entry<Models.DeviceRegistration>(oldDeviceRegistration).CurrentValues.SetValues(deviceRegistration);
            }

            _context.SaveChanges(true);
            _log.Log($"Dispositivo {deviceRegistration.Name} foi criado/alterado.");

            return deviceRegistration;
        }

        public void DeleteDeviceRegistration(string id)
        {
            Models.DeviceRegistration deviceRegistration = GetDeviceRegistration(id);
            if (deviceRegistration == null)
                return;

            _context.DevicesRegistration.Remove(deviceRegistration);
            _context.SaveChanges();
            _log.Log($"Dispositivo {deviceRegistration.Name} foi removido.");
        }

        public bool CanRegisterDevice(string deviceId)
        {
            return !_context.DevicesRegistration.Any(a => a.DeviceId == deviceId);
        }

        public DeviceRegistration ChangeSerialNumberByDevide(string deviceId, string serialNumber)
        {
            DeviceRegistration deviceRegistration = _context.DevicesRegistration.AsNoTracking().SingleOrDefault(c => c.DeviceId == deviceId);
            if (deviceRegistration == null)
                return null;

            deviceRegistration.SerialNumber = serialNumber;
            _context.Entry<DeviceRegistration>(deviceRegistration).State = EntityState.Modified;
            _context.SaveChanges(true);
            _log.Log($"Serial Number do dispositivo {deviceRegistration.DeviceId} foi criado/alterado.");

            return deviceRegistration;
        }

        public DeviceRegistration ChangeModelByDevide(string deviceId, string model)
        {
            DeviceRegistration deviceRegistration = _context.DevicesRegistration.AsNoTracking().SingleOrDefault(c => c.DeviceId == deviceId);
            if (deviceRegistration == null)
                return null;

            deviceRegistration.Model = model;
            _context.Entry<DeviceRegistration>(deviceRegistration).State = EntityState.Modified;
            _context.SaveChanges(true);
            _log.Log($"Modelo do dispositivo {deviceRegistration.DeviceId} foi criado/alterado.");

            return deviceRegistration;
        }

        public DeviceRegistration ChangeNotesByDevide(string deviceId, string notes)
        {
            DeviceRegistration deviceRegistration = _context.DevicesRegistration.AsNoTracking().SingleOrDefault(c => c.DeviceId == deviceId);
            if (deviceRegistration == null)
                return null;

            deviceRegistration.Notes = notes;
            deviceRegistration.NotesCreateDate = DateTime.Now;
            _context.Entry<DeviceRegistration>(deviceRegistration).State = EntityState.Modified;
            _context.SaveChanges(true);
            _log.Log($"Anotações do dispositivo {deviceRegistration.DeviceId} foi criado/alterado.");

            return deviceRegistration;
        }

        public DeviceRegistration ChangeFieldByDevide(string deviceId, string field, string value)
        {
            DeviceRegistration deviceRegistration = _context.DevicesRegistration.AsNoTracking().SingleOrDefault(c => c.DeviceId == deviceId);
            if (deviceRegistration == null)
                return null;
            
            if (string.IsNullOrWhiteSpace(field))
                return null;

            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (field.Trim().ToLower().Equals("ed1"))
                deviceRegistration.Ed1 = value;
            else if (field.Trim().ToLower().Equals("ed2"))
                deviceRegistration.Ed2 = value;
            else if (field.Trim().ToLower().Equals("ed3"))
                deviceRegistration.Ed3 = value;
            else if (field.Trim().ToLower().Equals("ed4"))
                deviceRegistration.Ed4 = value;
            else if (field.Trim().ToLower().Equals("sd1"))
                deviceRegistration.Sd1 = value;
            else if (field.Trim().ToLower().Equals("sd2"))
                deviceRegistration.Sd2 = value;
            else if (field.Trim().ToLower().Equals("ea10"))
                deviceRegistration.Ea10 = value;
            else if (field.Trim().ToLower().Equals("sa3"))
                deviceRegistration.Sa3 = value;
            
            _context.Entry<DeviceRegistration>(deviceRegistration).State = EntityState.Modified;
            _context.SaveChanges(true);
            _log.Log($"Anotações do dispositivo {deviceRegistration.DeviceId} foi criado/alterado.");

            return deviceRegistration;
        }
        
    }
}