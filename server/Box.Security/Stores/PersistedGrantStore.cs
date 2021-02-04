using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Box.Security.Stores
{
    /// <summary>
    /// Implements the persistedGrantStore.
    /// Not really used at the deafult implicit flow.
    /// </summary>
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IServiceProvider _provider;

        public PersistedGrantStore(IServiceProvider provider)
        {
            _provider = provider;
        }
        
        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {            
            var task = new Task<IEnumerable<PersistedGrant>>(() =>
                {
                    using (var context = _provider.GetService<Data.SecurityDbContext>())
                    {
                        return context.PersistedGrants.Where(g => g.SubjectId == subjectId).ToArray();
                    }
                });
            return task;            
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            var task = new Task<PersistedGrant>(() =>
            {
                using (var context = _provider.GetService<Data.SecurityDbContext>())
                {
                    return context.PersistedGrants.Find(key);
                }
            });
            return task;
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            var task = new Task(() =>
            {
                using (var context = _provider.GetService<Data.SecurityDbContext>())
                {
                    var grants = context.PersistedGrants.Where(g => g.SubjectId == subjectId && g.ClientId == clientId);
                    foreach (var g in grants)
                        context.Remove(g);
                    context.SaveChanges();
                }
            });
            return task;
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var task = new Task(() =>
            {
                using (var context = _provider.GetService<Data.SecurityDbContext>())
                {
                    var grants = context.PersistedGrants.Where(g => g.SubjectId == subjectId && g.ClientId == clientId && g.Type == type);
                    foreach (var g in grants)
                        context.Remove(g);
                    context.SaveChanges();
                }
            });
            return task;
        }

        public Task RemoveAsync(string key)
        {
            var task = new Task(() =>
            {
                using (var context = _provider.GetService<Data.SecurityDbContext>())
                {
                    var g = context.PersistedGrants.Find(key);
                    if (g == null)
                        return;
                    context.Remove(g);
                    context.SaveChanges();
                }
            });
            return task;
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            var task = new Task(() =>
            {
                using (var context = _provider.GetService<Data.SecurityDbContext>())
                {
                    context.Add(grant);
                    context.SaveChanges();
                }
            });
            return task;
        }
    }
}
