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
        public IEnumerable<Outgoing> GetOutgoings(int skip = 0, int top = 0, string filter = null, int month = 0, int year = 0, OptionalOutTotalCount totalCount = null)
        {
            IQueryable<Outgoing> outgoings = _context.Outgoings;

            if (month != 0)
                outgoings = outgoings.Where(c => c.Month == month);

            if (year != 0)
                outgoings = outgoings.Where(c => c.Year == year);

            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                outgoings = outgoings.Where(c =>
                    c.Description.ToLower().Contains(filter));
            }

            // ordernação
            outgoings = outgoings.OrderByDescending(c => c.Year).ThenByDescending(c => c.Month);

            if (totalCount != null)
                totalCount.Value = outgoings.Count();

            if (skip != 0)
                outgoings = outgoings.Skip(skip);

            if (top != 0)
                outgoings = outgoings.Take(top);

            return outgoings.ToArray();
        }
        public Outgoing GetOutgoing(string id)
        {
            return _context.Outgoings.Find(id);
        }

        public OutgoingViewModel GetOutgoingShow(string id)
        {
            OutgoingViewModel outgoingViewModel = new OutgoingViewModel();
            outgoingViewModel.Outgoing = _context.VW_Outgoings.SingleOrDefault(c => c.OutgoingUId == id);
            
            var clients = from c in _context.Clients
                            group c by new { c.Name } into cli
                            select new OutigoingClient
                            {
                                Name = cli.Key.Name,
                                Total = cli.Sum(s => Convert.ToDecimal(s.Value))
                            };

            outgoingViewModel.Clients = clients.ToArray();
            return outgoingViewModel;
        }

        public Outgoing GetOutgoingByCurrentYearMonth()
        {
            var today = DateTime.UtcNow.AddHours(-3);
            return _context.Outgoings.SingleOrDefault(c => c.Year == today.Year && c.Month == today.Month);
        }

        public Outgoing GetOutgoingByYearMonth(int year, int month)
        {
            return _context.Outgoings.SingleOrDefault(c => c.Year == year && c.Month == month);
        }

        public Outgoing SaveOutgoing(Outgoing outgoing)
        {
            Outgoing oldOutgoing = GetOutgoing(outgoing.OutgoingUId);
            if (oldOutgoing == null)
                oldOutgoing = GetOutgoingByYearMonth(outgoing.Year, outgoing.Month);

            if (oldOutgoing == null)
                _context.Entry<Outgoing>(outgoing).State = EntityState.Added;
            else
            {
                _context.Outgoings.Attach(oldOutgoing);
                _context.Entry<Outgoing>(oldOutgoing).CurrentValues.SetValues(outgoing);
            }

            _context.SaveChanges(true);
            _log.Log($"Configuração comercial do mês {outgoing.Month}, ano {outgoing.Year} foi criado/alterado.");

            return outgoing;
        }

        public void CalcClientsAndLicensesActived()
        {
            Outgoing oldOutgoing = GetOutgoingByCurrentYearMonth();
            var (clientsActived, devicesActived) = GetCliensAndDevicesActived();

            if (oldOutgoing == null)
            {
                var today = DateTime.UtcNow.AddHours(-3);
                Outgoing outgoing = new Outgoing()
                {
                    Year = today.Year,
                    Month = today.Month,
                    ClientsActive = clientsActived,
                    LicensesActive = devicesActived,
                    OutgoingUId = Guid.NewGuid().ToString()

                };
                _context.Entry<Outgoing>(outgoing).State = EntityState.Added;
                _context.SaveChanges(true);
                _log.Log($"Configuração comercial do mês {outgoing.Month}, ano {outgoing.Year} foi criado.");
            }
            else
            {
                oldOutgoing.ClientsActive = clientsActived;
                oldOutgoing.LicensesActive = devicesActived;
                _context.Entry<Outgoing>(oldOutgoing).State = EntityState.Modified;

                _context.SaveChanges(true);
                _log.Log($"Configuração comercial do mês {oldOutgoing.Month}, ano {oldOutgoing.Year} foi atualizada, cliente ativos {clientsActived} e dispositivos ativos {devicesActived}.");
            }
        }

        public void DeleteOutgoing(string id)
        {
            Outgoing outgoing = GetOutgoing(id);
            if (outgoing == null)
                return;

            _context.Outgoings.Remove(outgoing);
            _context.SaveChanges();
            _log.Log($"Configuração comercial do mês {outgoing.Month}, ano {outgoing.Year} foi removido.");
        }
    }
}