using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Box.Common.Web;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.ViewModels;

namespace SmartGeoIot.Services
{
    public partial class SmartGeoIotService
    {
        public IEnumerable<DashboardViewModels> GetGraphicsMonths(string id, ClaimsPrincipal user)
        {
            var startTime = DateTime.Now;
            List<DashboardViewModels> dashboards = new List<DashboardViewModels>();

            int currentMonth = DateTime.Now.Month;
            int start = currentMonth - 12;
            for (int i = start; i <= currentMonth; i++)
            {
                var deviceMessageType22 = _context.Messages.AsNoTracking().Include(inc => inc.Device)
                    .Where(w => w.DeviceId == id && w.Data.Substring(0, 2) == "22" && w.Date.Month == i)
                    .OrderByDescending(o => o.Id).Take(1).FirstOrDefault();

                var deviceMessageType23 = _context.Messages.AsNoTracking().Include(inc => inc.Device)
                      .Where(w => w.DeviceId == id && w.Data.Substring(0, 2) == "23" && w.Date.Month == i)
                      .OrderByDescending(o => o.Id).Take(1).FirstOrDefault();

                if (deviceMessageType22 != null && deviceMessageType23 != null)
                    dashboards.Add(CreateDashboard_Pack22_23ViewModel(deviceMessageType22, deviceMessageType23));
            }
            return dashboards.OrderByDescending(o => o.Date);
        }

        public IEnumerable<DashboardViewModels> GetGraphicsDays(string id, ClaimsPrincipal user)
        {
            var startTime = DateTime.Now;
            List<DashboardViewModels> dashboards = new List<DashboardViewModels>();

            int currentDay = DateTime.Now.Day;
            int start = currentDay - 7;
            for (int i = start; i <= currentDay; i++)
            {
                var deviceMessageType22 = _context.Messages.AsNoTracking().Include(inc => inc.Device)
                    .Where(w => w.DeviceId == id && w.Data.Substring(0, 2) == "22" && w.Date.Day == i)
                    .OrderByDescending(o => o.Id).Take(1).FirstOrDefault();

                var deviceMessageType23 = _context.Messages.AsNoTracking().Include(inc => inc.Device)
                      .Where(w => w.DeviceId == id && w.Data.Substring(0, 2) == "23" && w.Date.Day == i)
                      .OrderByDescending(o => o.Id).Take(1).FirstOrDefault();

                if (deviceMessageType22 != null && deviceMessageType23 != null)
                    dashboards.Add(CreateDashboard_Pack22_23ViewModel(deviceMessageType22, deviceMessageType23));
            }
            return dashboards.OrderByDescending(o => o.Date);
        }

    }
}