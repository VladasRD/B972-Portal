using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Box.Common.Web;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.Extensions;
using SmartGeoIot.Models;

namespace SmartGeoIot.Services
{
    public partial class SmartGeoIotService
    {
        public IEnumerable<Device> GetDevices(ClaimsPrincipal user, int skip = 0, int top = 0, string filter = null, OptionalOutTotalCount totalCount = null, bool isFullAcess = false, string scope = null)
        {
            IQueryable<Device> devicesQuery = _context.Devices.Where(w => w.Activable);

            if (!isFullAcess)
            {
                // filtrar apenas os dispositivos que tem acesso
                var userDevices = GetUserDevices(user.GetId());
                devicesQuery = devicesQuery.Where(c => userDevices.Any(a => a.Id == c.Id));
            }

            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                devicesQuery = devicesQuery.Where(f => f.Name.ToLower().Contains(filter) || f.Id.ToLower().Contains(filter));
            }

            // ordernação
            devicesQuery = devicesQuery.OrderBy(o => o.Id);
            if (totalCount != null)
                totalCount.Value = devicesQuery.Count();

            if (skip != 0)
                devicesQuery = devicesQuery.Skip(skip);

            if (top != 0)
                devicesQuery = devicesQuery.Take(top);

            var devices = devicesQuery.ToArray();

            if (devices != null && scope == null)
            {
                foreach (var d in devices)
                {
                    // adicionar dados da "message" para mostrar previamente nas telas de listagem algumas informações
                    var dashboard = GetDashboard(d.Id);
                    if (dashboard == null)
                        d.Bits = new ViewModels.Bits();
                    else
                        d.Bits = dashboard.Bits;
                }
            }

            return devices;
        }

        public IEnumerable<DeviceRegistration> GetDevicesWithDataFirmware(ClaimsPrincipal user, int skip = 0, int top = 0, string filter = null, string clientUId = null, OptionalOutTotalCount totalCount = null)
        {
            var devicesIdWithFirmware = _context.Messages.Where(c => c.TypePackage.Equals("51") || c.TypePackage.Equals("52")).Select(s => s.DeviceId);
            IQueryable<DeviceRegistration> devicesQuery = _context.DevicesRegistration
            .Include(i => i.Device)
            .Where(w => w.Device.Activable && devicesIdWithFirmware.Any(a => a == w.DeviceId));

            if (clientUId != null && !String.IsNullOrEmpty(clientUId) && clientUId != "null")
            {
                var devicesClient = _context.ClientsDevices.Where(c => c.ClientUId == clientUId);
                devicesQuery = devicesQuery.Where(c => devicesClient.Any(a => a.Id == c.DeviceId));
            }

            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                devicesQuery = devicesQuery.Where(f => f.Name.ToLower().Contains(filter) || f.DeviceId.ToLower().Contains(filter));
            }

            // ordernação
            devicesQuery = devicesQuery.OrderBy(o => o.DeviceId);
            if (totalCount != null)
                totalCount.Value = devicesQuery.Count();

            if (skip != 0)
                devicesQuery = devicesQuery.Skip(skip);

            if (top != 0)
                devicesQuery = devicesQuery.Take(top);

            return devicesQuery.ToArray();
        }

        public IEnumerable<DeviceRegistration> GetDevicesFromDashboard(ClaimsPrincipal user, int skip = 0, int top = 0, string filter = null, OptionalOutTotalCount totalCount = null, bool isFullAcess = false)
        {
            IQueryable<DeviceRegistration> devicesQuery = _context.DevicesRegistration.Include(i => i.Device).Include(i => i.Package).Include(i => i.Project).Where(w => w.Device.Activable);

            if (!isFullAcess)
            {
                // filtrar apenas os dispositivos que tem acesso
                var userDevices = GetUserDevices(user.GetId());
                devicesQuery = devicesQuery.Where(c => userDevices.Any(a => a.Id == c.DeviceId));
            }

            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                devicesQuery = devicesQuery.Where(f => f.Name.ToLower().Contains(filter) || f.DeviceId.ToLower().Contains(filter));
            }

            // ordernação
            devicesQuery = devicesQuery.OrderBy(o => o.DeviceId);
            if (totalCount != null)
                totalCount.Value = devicesQuery.Count();

            if (skip != 0)
                devicesQuery = devicesQuery.Skip(skip);

            if (top != 0)
                devicesQuery = devicesQuery.Take(top);

            var devices = devicesQuery.ToArray();
            if (devices != null)
            {
                foreach (var d in devices)
                {
                    // adicionar dados da "message" para mostrar previamente nas telas de listagem algumas informações
                    var dashboard = GetDashboard(d.DeviceId);
                    if (dashboard == null)
                        d.Device.Bits = new ViewModels.Bits();
                    else
                        d.Device.Bits = dashboard.Bits;
                }
            }
            return devices;
        }

        public IEnumerable<DeviceRegistration> GetDevicesOfClient(ClaimsPrincipal user, bool isFullAcess = false, string project = null)
        {
            IQueryable<DeviceRegistration> devices = _context.DevicesRegistration.Where(w => w.Device.Activable);

            if (!isFullAcess)
            {
                // filtrar apenas os dispositivos que tem acesso
                var userDevices = GetUserDevices(user.GetId());
                devices = devices.Where(c => userDevices.Any(a => a.Id == c.DeviceId));
            }

            if (project != null)
                devices = devices.Where(c => c.Package.Type.Equals(project));

            // ordernação
            devices = devices.OrderBy(o => o.DeviceId);

            return devices.ToArray();
        }

        public Device GetDevice(string id)
        {
            return _context.Devices.Find(id);
        }

        // Retorna os dispositivos que o usuário tem acesso
        public IEnumerable<ClientDevice> GetUserDevices(string userId)
        {
            var clientUsers = _context.ClientsUsers.Where(c => c.ApplicationUserId == userId);
            var userDevices = _context.ClientsDevices.Where(a => clientUsers.Any(b => b.ClientUId == a.ClientUId));
            return userDevices.ToArray();
        }

    }
}