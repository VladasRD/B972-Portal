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
    public partial class RadiodadosService
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
            IQueryable<DeviceRegistration> devicesQuery = _context.DevicesRegistration
                .Include(i => i.Device)
                .Include(i => i.Package)
                .Include(i => i.Project)
                .Where(w => w.Device.Activable && w.Project.Code != Utils.EnumToAnnotationText(ProjectCode.B975));

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
            // if (devices != null)
            // {
            //     foreach (var d in devices)
            //     {
            //         // adicionar dados da "message" para mostrar previamente nas telas de listagem algumas informações
            //         var dashboard = GetDashboard(d.DeviceId);
            //         if (dashboard == null)
            //             d.Device.Bits = new ViewModels.Bits();
            //         else
            //             d.Device.Bits = dashboard.Bits;
            //     }
            // }
            return devices;
        }

        public IEnumerable<ViewModels.B975DevicesDashboardViewModels> GetDevicesB975FromDashboard(ClaimsPrincipal user, int skip = 0, int top = 0, string filter = null, OptionalOutTotalCount totalCount = null, bool isFullAcess = false, bool filtrarBloqueios = false)
        {
            IQueryable<DeviceRegistration> devicesQuery = _context.DevicesRegistration
                .Where(w => w.Device.Activable && w.Project.Code == Utils.EnumToAnnotationText(ProjectCode.B975));

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

            // var devices = devicesQuery.ToArray();
            List<ViewModels.B975DevicesDashboardViewModels> query = new List<ViewModels.B975DevicesDashboardViewModels>();
            if (devicesQuery != null)
            {
                ServiceDesk[] serviceDesks = _context.ServiceDesks.Where(c => devicesQuery.Any(a => a.DeviceId == c.DeviceId)).ToArray();
                // B975[] b975s = _context
                // fazer aqui a chamada para recuperar o ultimo dado de cada device

                foreach (var d in devicesQuery)
                {
                    // adicionar dados da "message" para mostrar previamente nas telas de listagem algumas informações
                    B975 lastB975 = GetLastB975ByDevice(d.DeviceId, filtrarBloqueios);
                    if (filtrarBloqueios && lastB975 != null)
                    {
                        if (lastB975.StatusDJ == null)
                            continue;

                        if (!lastB975.StatusDJ.Equals(Utils.GetStatusDJName(4)))
                            continue;
                    }

                    if (filtrarBloqueios && lastB975 == null)
                        continue;
                    query.Add(
                        new ViewModels.B975DevicesDashboardViewModels()
                        {
                            DeviceId = d.DeviceId,
                            Name = d.Name,
                            NickName = d.NickName,
                            Date = (lastB975 != null) ? lastB975?.DateGMTBrasilian : null,
                            StatusDJ = (lastB975 != null) ? lastB975?.StatusDJ : null,
                            ContadorCarencias = (lastB975 != null) ? lastB975?.ContadorCarencias : null,
                            ContadorBloqueios = (lastB975 != null) ? lastB975?.ContadorBloqueios : null,
                            LocationCity = (lastB975 != null) ? lastB975?.LocationCity : null,
                            HasServiceDeskOpened = serviceDesks.Any(c => c.DeviceId == d.DeviceId && c.IsOpened),
                            HasHistoryServiceDesk = serviceDesks.Any(c => c.DeviceId == d.DeviceId)
                        }
                    );
                }
            }
            
            var _result = query.OrderBy(c => c.NickName).OrderByDescending(c => c.Date).ToArray();
            return _result;
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

            if (project != null && project != "null")
                devices = devices.Where(c => c.Project.Code.ToLower() == project.ToLower());

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

        public void SaveLastStatusDevice(string deviceId)
        {
            DeviceRegistration devicesQuery = _context.DevicesRegistration
                .Include(i => i.Device)
                .Include(i => i.Package)
                .Include(i => i.Project)
                .SingleOrDefault(w => w.Device.Activable && w.DeviceId == deviceId);

            if (devicesQuery.Project.Code == Utils.EnumToAnnotationText(ProjectCode.B987))
            {

            }


        }

    }
}