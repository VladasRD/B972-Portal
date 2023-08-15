using System;
using System.Collections.Generic;
using System.Linq;
using Box.Common.Web;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.Models;

namespace SmartGeoIot.Services
{
    public partial class RadiodadosService
    {
        public IEnumerable<Package> GetPackages(int skip = 0, int top = 0, string filter = null, OptionalOutTotalCount totalCount = null)
        {
            IQueryable<Package> packages = _context.Packages;

            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                packages = packages.Where(c =>
                    c.Name.ToLower().Contains(filter) ||
                    c.Type.ToLower().Contains(filter) ||
                    c.Description.ToLower().Contains(filter));
            }

            // ordernação
            packages = packages.OrderBy(c => c.Name);

            if (totalCount != null)
                totalCount.Value = packages.Count();

            if (skip != 0)
                packages = packages.Skip(skip);

            if (top != 0)
                packages = packages.Take(top);

            return packages.ToArray();
        }
        public Package GetPackage(string id)
        {
            return _context.Packages.Find(id);
        }

        public Models.Package SavePackage(Models.Package package)
        {
            Models.Package oldPackage = GetPackage(package.PackageUId);

            if (oldPackage == null)
                _context.Entry<Models.Package>(package).State = EntityState.Added;
            else
            {
                _context.Packages.Attach(oldPackage);
                _context.Entry<Models.Package>(oldPackage).CurrentValues.SetValues(package);
            }

            _context.SaveChanges(true);
            // _log.Log($"Pacote {package.Name} foi criado/alterado.");

            return package;
        }

        public void DeletePackage(string id)
        {
            Models.Package package = GetPackage(id);
            if (package == null)
                return;

            _context.Packages.Remove(package);
            _context.SaveChanges();
            // _log.Log($"Pacote {package.Name} foi removido.");
        }
        
    }
}