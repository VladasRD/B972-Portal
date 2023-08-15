using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.Extensions;
using SmartGeoIot.Models;

namespace SmartGeoIot.Services
{
    public partial class RadiodadosService
    {
        public StandardPagedResponse<IEnumerable<B979ViewModel>> GetAPIListB979(string apiKey, StandardPagedResponse<IEnumerable<B979ViewModel>> response, int skip = 0, int top = 0, string deviceId = null, string initialDate = null, string finalDate = null)
        {
            var clientDevices = _context.Clients.Include(i => i.Devices).SingleOrDefault(c => c.Active && c.ApiKey == apiKey);
            if (clientDevices == null)
            {
                response.Data = null;
                response.MessageToUser = "Não encontramos informações para os dados informados.";
                response.MessageToUser = "Error";
                return response;
            }

            ClientDevice[] devices = clientDevices.Devices.Where(c => c.Active).ToArray();
            if (deviceId != null)
                devices = devices.Where(c => c.Id == deviceId).ToArray();

            IQueryable<B979> b979s = _context.B979s.Where(c => devices.Any(a => a.Id == c.DeviceId));
            if (initialDate != null)
            {
                DateTime firstDate = Convert.ToDateTime(initialDate).ToUniversalTime();
                b979s = b979s.Where(c => c.Data.Year >= firstDate.Year && c.Data.Month >= firstDate.Month && c.Data.Day >= firstDate.Day);
            }
            if (finalDate != null)
            {
                DateTime lastDate = Convert.ToDateTime(finalDate).ToUniversalTime();
                b979s = b979s.Where(c => c.Data.Year <= lastDate.Year && c.Data.Month <= lastDate.Month && c.Data.Day <= lastDate.Day);
            }

            response.TotalItensOfRequest = b979s.Count();
            if (skip != 0)
                b979s = b979s.Skip(skip);

            if (top != 0)
                b979s = b979s.Take(top);

            response.Data = b979s.ToArray().Select(s => new B979ViewModel
            {
                DeviceId = s.DeviceId
                ,Data = s.Data
                ,Acel = s.Acel
                ,Desacel = s.Desacel
                ,EncoderPMA = s.EncoderPMA
                ,EncoderPMF = s.EncoderPMF
                ,TimerFreioOn = s.TimerFreioOn
                ,TimerFreioOff = s.TimerFreioOff
                ,Timer = s.Timer
                ,TimerP2 = s.TimerP2
                ,TOVelBaixa = s.TOVelBaixa
                ,TempoPMA = s.TempoPMA
                ,TempoPMF = s.TempoPMF
                ,VelBaixa = s.VelBaixa
                ,VelAltaAbrir = s.VelAltaAbrir
                ,VelAltaFechar = s.VelAltaFechar
                ,Ciclos = s.Ciclos
                ,Horimetro = s.Horimetro
                ,Inversor = s.Inversor
                ,Estado = Utils.EnumToAnnotationText((enumState)s.Estado)
            }).OrderBy(o => o.Data).ToArray();

            response.TotalPages = top==0 ? 0 : (int)Math.Ceiling((double)response.TotalItensOfRequest / top);
            response.PageNumber = skip;
            response.ItemsOnThisPage = top;
            return response;
        }

        public async Task SaveB979RequestToDevice(B979RequestToDevice request)
        {
            _context.B979RequestToDevices.Add(request);
            await _context.SaveChangesAsync();
        }
    }
}