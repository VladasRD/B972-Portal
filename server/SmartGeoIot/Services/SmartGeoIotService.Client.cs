using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Box.Common.Web;
using Box.Security.Models;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.Extensions;
using SmartGeoIot.Models;
using SmartGeoIot.ViewModels;

namespace SmartGeoIot.Services
{
    public partial class SmartGeoIotService
    {
        public IEnumerable<Client> GetClients(ClaimsPrincipal user, int skip = 0, int top = 0, string filter = null, bool? statusClient = null, bool isSubClient = false, OptionalOutTotalCount totalCount = null)
        {
            IQueryable<Client> clients = _context.Clients;

            if (isSubClient)
            {
                var clientsOfUser = _context.Clients.Include(i => i.Users).Where(c => c.Users.Any(a => a.ApplicationUserId == user.GetId()));
                clients = clients.Where(c => clientsOfUser.Any(a => a.ClientUId == c.ClientFatherUId));
            }

            if (statusClient != null)
                clients = clients.Where(c => c.Active == statusClient);

            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                clients = clients.Where(c =>
                    c.Name.ToLower().Contains(filter) ||
                    c.Document.ToLower().Contains(filter) ||
                    c.Address.ToLower().Contains(filter)
                    );
            }

            // ordernação
            clients = clients.OrderBy(c => c.Name);

            if (totalCount != null)
                totalCount.Value = clients.Count();

            if (skip != 0)
                clients = clients.Skip(skip);

            if (top != 0)
                clients = clients.Take(top);

            return clients.ToArray();
        }

        public Client GetClientByDevice(string deviceId)
        {
            return _context.Clients.Include(i => i.Devices).FirstOrDefault(s => s.Devices.Any(c => c.Id == deviceId));
        }
        public Client GetClient(string id)
        {
            var client = _context.Clients.AsNoTracking()
                .Include(b => b.Devices).ThenInclude(t => t.AppDevice)
                .Include(ius => ius.Users)
                .Include(i => i.Billings)
                .SingleOrDefault(w => w.ClientUId == id);

            if (client != null)
            {
                // ordernação
                client.Users = client.Users.OrderBy(o => o.ClientUId).ToList();

                if (client.Users != null)
                {
                    if (client.Users.Count > 0)
                    {
                        var users = _securityContext.Users.Include(u => u.UserClaims).Where(u => client.Users.Any(s => s.ApplicationUserId == u.Id)).ToArray();

                        List<ClientUser> removeUserDontExist = new List<ClientUser>();
                        foreach (var s in client.Users)
                        {
                            var user = users.SingleOrDefault(u => u.Id == s.ApplicationUserId);
                            if (user != null)
                            {
                                s.AppUser = user;
                                s.AppUser.RemoveNoIdentityClaims();
                            }
                            else
                                removeUserDontExist.Add(s);
                        }

                        foreach (var user in removeUserDontExist)
                        {
                            client.Users.Remove(user);
                        }
                    }
                }

                client.Billings = client.Billings.OrderByDescending(o => o.PaymentDueDate).ToArray();
            }

            return client;
        }

        public Client GetClientOfUser(string userId)
        {
            return _context.Clients.Include(i => i.Users).FirstOrDefault(c => c.Users.Any(a => a.ApplicationUserId == userId));
        }

        public Client SaveClient(Client client, ClaimsPrincipal _user, bool isSubClient = false)
        {
            Client oldClient = GetClient(client.ClientUId);

            if (isSubClient)
            {
                var clientOfUser = GetClientOfUser(_user.GetId());
                client.ClientFatherUId = clientOfUser.ClientUId;
            }

            if (oldClient == null)
            {
                _context.Entry<Client>(client).State = EntityState.Added;
            }
            else
            {
                _context.Clients.Attach(oldClient);
                _context.Entry<Client>(oldClient).CurrentValues.SetValues(client);

                // Removendo relacionamento de dispositivos
                if (oldClient.Devices != null)
                {
                    _context.ClientsDevices.RemoveRange(oldClient.Devices);
                }

                // Removendo relacionamento de usuários
                if (oldClient.Users != null)
                {
                    _context.ClientsUsers.RemoveRange(oldClient.Users);
                }
            }

            if (client.Devices != null)
            {
                foreach (var device in client.Devices)
                {
                    device.ClientDeviceUId = Guid.NewGuid().ToString();

                    if (oldClient != null)
                        device.ClientUId = oldClient.ClientUId;
                    else
                        device.ClientUId = client.ClientUId;

                    // desabilitando para não inserir na view e dar erro de identity insert
                    device.AppDevice = null;
                    _context.ClientsDevices.Add(device);
                }
            }

            // adicionando os vendedores
            if (client.Users != null)
            {
                foreach (var user in client.Users)
                {
                    user.ClientUserUId = Guid.NewGuid().ToString();

                    if (oldClient != null)
                        user.ClientUId = oldClient.ClientUId;
                    else
                        user.ClientUId = client.ClientUId;

                    // desabilitando para não inserir e dar erro de identity insert
                    user.AppUser = null;
                    _context.ClientsUsers.Add(user);
                }
            }

            _context.SaveChanges(true);
            _log.Log($"Cliente {client.Name} foi criado/alterado.");

            return client;
        }

        public void DisableClient(string id)
        {
            Client client = GetClient(id);
            if (client == null)
                return;

            client.Active = false;
            _context.Entry<Client>(client).State = EntityState.Modified;
            _context.SaveChanges();
            _log.Log($"Clientes {client.Name} foi desativado.");
        }

        public bool HasBillingCurrentMonth(string clientUId, int month)
        {
            return _context.ClientsBillings.Any(c => c.ClientUId == clientUId && c.Create.Month.Equals(month));
        }

        public ClientBilling CreateBilling(Client client)
        {
            var clientBilling = new ClientBilling()
            {
                ClientBillingUId = Guid.NewGuid().ToString(),
                ClientUId = client.ClientUId,
                Create = DateTime.Now,
                PaymentDueDate = Convert.ToDateTime($"{client.DueDay}-{DateTime.Now.Month}-{DateTime.Now.Year}"),
                PaymentDate = null,
                Sended = false
            };

            // criar o faturamento na gerencia-net
            GerenciaNetViewModels.ChargeResponse chargeResponse = CreateBillet(client, clientBilling);
            
            // verificar se sucesso, pode gravar o billing
            if (chargeResponse.code.Equals(200))
            {
                clientBilling.ExternalId = chargeResponse.data.charge_id;
                clientBilling.BarCode = chargeResponse.data.barcode;
                clientBilling.LinkPdf = chargeResponse.data.pdf.charge;
                clientBilling.Status = chargeResponse.data.status;
                clientBilling.Sended = true;
                _context.ClientsBillings.Add(clientBilling);
                _context.SaveChanges();
                _log.Log($"Faturamento do cliente {client.Name} com vencimento em {clientBilling.PaymentDueDate.Value.ToShortDateString()} foi criando.");

                return clientBilling;
            }

            return null;
        }

        public IEnumerable<Client> GetClientsByDevice(string deviceId)
        {
            return _context.Clients
            .Include(i => i.Devices)
            .AsNoTracking()
            .Where(c => c.Devices.Any(a => a.Id == deviceId && a.Active) && c.Active);
        }

        public void AddClientUser(string id, ApplicationUser user)
        {
            ClientUser oldClientUser = _context.ClientsUsers.SingleOrDefault(c => c.ClientUId == id && c.ApplicationUserId == user.Id);
            if (oldClientUser != null)
                return;

            ClientUser clientUser = new ClientUser()
            {
                ClientUserUId = Guid.NewGuid().ToString(),
                ClientUId = id,
                ApplicationUserId = user.Id,
                AppUser = null
            };

            _context.ClientsUsers.Add(clientUser);
            _context.SaveChanges(true);
            _log.Log($"Usuário {user.Email} foi vinculado ao cliente.");
        }

        public void RemoveClientUser(ClientUser clientUser)
        {
            _context.ClientsUsers.Remove(clientUser);
            _context.SaveChanges();
            _log.Log($"Usuário {clientUser.AppUser.Email} foi removido.");
        }

        public (int clientsActived, int devicesActived) GetCliensAndDevicesActived()
        {
            int clientsActived = _context.Clients.Where(c => c.Active).Count();
            int devicesActived = _context.Clients.Include(i => i.Devices).Where(c => c.Active && c.Devices.Any(a => a.Active)).Count();
            return (clientsActived, devicesActived);
        }

        // public void UpdateBillingSended(Client client, ClientBilling clientBilling)
        // {
        //     // var oldClientBilling = clientBilling;
        //     clientBilling.Sended = true;
        //     _context.Entry<ClientBilling>(clientBilling).State = EntityState.Modified;
        //     // _context.ClientsBillings.Attach(oldClientBilling);
        //     // _context.Entry<ClientBilling>(oldClientBilling).CurrentValues.SetValues(clientBilling);
        //     _context.SaveChanges();

        //     _log.Log($"Faturamento do cliente {client.Name} com vencimento em {clientBilling.PaymentDueDate} foi enviado.");
        // }


    }
}