using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Box.Common.Web;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.Models;
using SmartGeoIot.ViewModels;

namespace SmartGeoIot.Services
{
    public partial class SmartGeoIotService
    {
        public IEnumerable<DeviceRegistration> GetDevicesRegistrations(int skip = 0, int top = 0, string filter = null, OptionalOutTotalCount totalCount = null)
        {
            IQueryable<DeviceRegistration> devicesRegistrations = _context.DevicesRegistration.Include(i => i.Device).Include(i => i.Package).Include(i => i.Project);

            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                devicesRegistrations = devicesRegistrations.Where(c =>
                    c.Name.ToLower().Contains(filter));
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
            return _context.DevicesRegistration.Include(i => i.Package).SingleOrDefault(r => r.DeviceId == id);
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
        
    }
}