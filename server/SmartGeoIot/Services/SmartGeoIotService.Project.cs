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
        public IEnumerable<Project> GetProjects(int skip = 0, int top = 0, string filter = null, OptionalOutTotalCount totalCount = null)
        {
            IQueryable<Project> projects = _context.Projects;

            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                projects = projects.Where(c =>
                    c.Name.ToLower().Contains(filter) ||
                    c.Description.ToLower().Contains(filter));
            }

            // ordernação
            projects = projects.OrderBy(c => c.Name);

            if (totalCount != null)
                totalCount.Value = projects.Count();

            if (skip != 0)
                projects = projects.Skip(skip);

            if (top != 0)
                projects = projects.Take(top);

            return projects.ToArray();
        }

        public IEnumerable<Project> GetMeProjects(ClaimsPrincipal user, bool isFullAcess = false)
        {
            if (isFullAcess)
                return _context.Projects.ToArray();

            var clients = _context.Clients.Where(c => c.Users.Any(a => a.ApplicationUserId == user.GetId()));
            var deviceRegistrations = _context.DevicesRegistration.Include(i => i.Project).Where(c => clients.Any(a => a.Devices.Any(b => b.Id == c.DeviceId)));
            return deviceRegistrations.Select(s => s.Project).ToArray();
        }
        
        public Project GetProject(string id)
        {
            return _context.Projects.Find(id);
        }

        public Models.Project SaveProject(Models.Project project)
        {
            Models.Project oldProject = GetProject(project.ProjectUId);

            if (oldProject == null)
                _context.Entry<Models.Project>(project).State = EntityState.Added;
            else
            {
                _context.Projects.Attach(oldProject);
                _context.Entry<Models.Project>(oldProject).CurrentValues.SetValues(project);
            }

            _context.SaveChanges(true);
            _log.Log($"Projeto {project.Name} foi criado/alterado.");

            return project;
        }

        public void DeleteProject(string id)
        {
            Models.Project project = GetProject(id);
            if (project == null)
                return;

            _context.Projects.Remove(project);
            _context.SaveChanges();
            _log.Log($"Projeto {project.Name} foi removido.");
        }
        
    }
}