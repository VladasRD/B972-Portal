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
        public IEnumerable<ServiceDesk> GetServiceDesks(int skip = 0, int top = 0, string filter = null, OptionalOutTotalCount totalCount = null)
        {
            IQueryable<ServiceDesk> serviceDesk = _context.ServiceDesks;

            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                serviceDesk = serviceDesk.Where(c =>
                    c.DeviceId.ToLower().Contains(filter) ||
                    c.Status.ToLower().Contains(filter));
            }

            // ordernação
            serviceDesk = serviceDesk.OrderBy(c => c.DeviceId);

            if (totalCount != null)
                totalCount.Value = serviceDesk.Count();

            if (skip != 0)
                serviceDesk = serviceDesk.Skip(skip);

            if (top != 0)
                serviceDesk = serviceDesk.Take(top);

            return serviceDesk.ToArray();
        }

        public ServiceDesk GetLastServiceDeskOpened(string deviceId)
        {
            return _context.ServiceDesks.OrderByDescending(c => c.CreateDate).FirstOrDefault(c => c.DeviceId == deviceId && c.IsOpened);
        }

        // public Models.ServiceDesk SaveServiceDesk(Models.ServiceDesk serviceDesk)
        // {
        //     Models.ServiceDesk oldServiceDesk = GetServiceDesk(serviceDesk.ServiceDeskId);

        //     if (oldServiceDesk == null)
        //         _context.Entry<Models.ServiceDesk>(serviceDesk).State = EntityState.Added;
        //     else
        //     {
        //         _context.ServiceDesks.Attach(oldServiceDesk);
        //         _context.Entry<Models.ServiceDesk>(oldServiceDesk).CurrentValues.SetValues(serviceDesk);
        //     }

        //     _context.SaveChanges(true);
        //     _log.Log($"Chamado do dispositivo {serviceDesk.DeviceId} foi criado/alterado.");

        //     return serviceDesk;
        // }
        internal void CreateServiceDeskRecord(int serviceDeskId, string reason, string package, long? packageTimestamp)
        {
            ServiceDeskRecord serviceDeskRecord = new ServiceDeskRecord()
            {
                ServiceDeskId = serviceDeskId,
                Package = package,
                PackageTimestamp = packageTimestamp,
                Description = reason,
                CreateDate = DateTime.Now
            };

            _context.Entry<Models.ServiceDeskRecord>(serviceDeskRecord).State = EntityState.Added;
            _context.SaveChanges(true);
        }

        internal ServiceDesk CreateServiceDesk(string deviceId)
        {
            ServiceDesk serviceDesk = new ServiceDesk()
            {
                DeviceId = deviceId,
                CreateDate = DateTime.Now
            };

            _context.Entry<Models.ServiceDesk>(serviceDesk).State = EntityState.Added;
            _context.SaveChanges(true);

            return GetLastServiceDeskOpened(deviceId);
        }

        public void SaveServiceDeskByDashboard(string deviceId, string reason, string package = null, long? packageTimestamp = null)
        {
            ServiceDesk lastServiceDesk;
            lastServiceDesk = GetLastServiceDeskOpened(deviceId);

            if (lastServiceDesk == null)
                lastServiceDesk = CreateServiceDesk(deviceId);

            CreateServiceDeskRecord(lastServiceDesk.ServiceDeskId, reason, package, packageTimestamp.Value);
        }

        public void CloseServiceDesk(string deviceId, string reason)
        {
            ServiceDesk lastServiceDesk = GetLastServiceDeskOpened(deviceId);
            CreateServiceDeskRecord(lastServiceDesk.ServiceDeskId, reason, null, null);

            lastServiceDesk.FinishDate = DateTime.Now;

            _context.Entry<Models.ServiceDesk>(lastServiceDesk).State = EntityState.Modified;
            _context.SaveChanges(true);
        }

        public IEnumerable<ServiceDeskRecord> GetServiceDeskHistory(string deviceId, int skip = 0, int top = 0, string filter = null, OptionalOutTotalCount totalCount = null)
        {
            var serviceDesks = _context.ServiceDesks.Where(c => c.DeviceId == deviceId);
            IQueryable<ServiceDeskRecord> query = _context.ServiceDeskRecords
                .Where(c => serviceDesks.Any(r => r.ServiceDeskId == c.ServiceDeskId));

            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                query = query.Where(c =>
                    c.Description.ToLower().Contains(filter));
            }

            // ordernação
            query = query.OrderByDescending(c => c.CreateDate);

            if (totalCount != null)
                totalCount.Value = query.Count();

            if (skip != 0)
                query = query.Skip(skip);

            if (top != 0)
                query = query.Take(top);
            
            return query;
        }
    }
}