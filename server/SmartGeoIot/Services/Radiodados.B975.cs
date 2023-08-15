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
        public async Task ProcessDataB975(Models.Message message, Models.DeviceLocation deviceLocation)
        {
            B975 currentB975 = null;
            string letterPack = message.TypePackage.Substring(1,1);
            if (VerifyExistDataB975(message.DeviceId, letterPack, message.Data, message.Time))
                return;
            
            var oldPacksNull = GetB975(message.DeviceId, letterPack);
            if (oldPacksNull == null || oldPacksNull.Count() == 0)
            {
                currentB975 = new B975();
                BindPack(letterPack, message, currentB975, deviceLocation);

                await SaveB975(currentB975);
            }
            else
            {
                foreach (var old in oldPacksNull)
                {
                    currentB975 = old;
                    BindPack(letterPack, message, currentB975, deviceLocation);

                    await UpdateB975(currentB975);
                }
            }
        }

        internal void BindPack(string letterPack, Message message, B975 currentB975, DeviceLocation deviceLocation)
        {
            if (!letterPack.Equals("A"))
            {
                var lastPack = GetB975LastPack(deviceLocation.DeviceId, letterPack);
                if (lastPack != null)
                {
                    currentB975.PackA = lastPack.PackA;
                    currentB975.TimeA = lastPack.TimeA;
                    currentB975.PcPosChave = lastPack.PcPosChave;
                    currentB975.Jam = lastPack.Jam;
                    currentB975.Vio = lastPack.Vio;
                    currentB975.RasIn = lastPack.RasIn;
                    currentB975.Bloqueio = lastPack.Bloqueio;
                    currentB975.RasOut = lastPack.RasOut;
                    currentB975.StatusDJ = lastPack.StatusDJ;
                    currentB975.AlertaFonteBaixa = lastPack.AlertaFonteBaixa;
                    currentB975.IntervaloUpLink = lastPack.IntervaloUpLink;
                    currentB975.ContadorCarencias = lastPack.ContadorCarencias;
                    currentB975.ContadorBloqueios = lastPack.ContadorBloqueios;
                }
            }
            
            if (!letterPack.Equals("B"))
            {
                var lastPack = GetB975LastPack(deviceLocation.DeviceId, letterPack);
                if (lastPack != null)
                {
                    currentB975.PackB = lastPack.PackB;
                    currentB975.TimeB = lastPack.TimeB;
                    currentB975.TemperaturaInterna = lastPack.TemperaturaInterna;
                    currentB975.TensaoAlimentacao = lastPack.TensaoAlimentacao;
                }
            }
            
            if (!letterPack.Equals("C"))
            {
                var lastPack = GetB975LastPack(deviceLocation.DeviceId, letterPack);
                if (lastPack != null)
                {
                    currentB975.PackC = lastPack.PackC;
                    currentB975.TimeC = lastPack.TimeC;
                    currentB975.MediaRFMinimo = lastPack.MediaRFMinimo;
                    currentB975.MediaRFMaximo = lastPack.MediaRFMaximo;
                    currentB975.MediaLinhaBase = lastPack.MediaLinhaBase;
                    currentB975.MediaInterferencia = lastPack.MediaInterferencia;
                    currentB975.DeteccaoInterferencia = lastPack.DeteccaoInterferencia;
                    currentB975.DeteccaoJammer = lastPack.DeteccaoJammer;
                    currentB975.NumeroViolacao = lastPack.NumeroViolacao;
                }
            }

            if (letterPack.Equals("A"))
            {
                currentB975.PackA = message.Data;
                currentB975.TimeA = message.Time;
                currentB975.PcPosChave = message.Bits.PcPosChave;
                currentB975.Jam = message.Bits.Jam;
                currentB975.Vio = message.Bits.Vio;
                currentB975.RasIn = message.Bits.RasIn;
                currentB975.Bloqueio = message.Bits.Bloqueio;
                currentB975.RasOut = message.Bits.RasOut;
                currentB975.StatusDJ = Utils.GetStatusDJName(message.StatusDJ);
                currentB975.AlertaFonteBaixa = message.Bits.AlertaFonteBaixa;
                currentB975.IntervaloUpLink = message.IntervaloUpLink;
                currentB975.ContadorCarencias = message.ContadorCarenciasB975;
                currentB975.ContadorBloqueios = message.ContadorBloqueiosB975;
            }
            else if (letterPack.Equals("B"))
            {
                currentB975.PackB = message.Data;
                currentB975.TimeB = message.Time;
                currentB975.TemperaturaInterna = message.TemperaturaInterna;
                currentB975.TensaoAlimentacao = message.TensaoAlimentacao;
            }
            else if (letterPack.Equals("C"))
            {
                currentB975.PackC = message.Data;
                currentB975.TimeC = message.Time;
                currentB975.MediaRFMinimo = message.MediaRFMinimo;
                currentB975.MediaRFMaximo = message.MediaRFMaximo;
                currentB975.MediaLinhaBase = message.MediaLinhaBase;
                currentB975.MediaInterferencia = message.MediaInterferencia;
                currentB975.DeteccaoInterferencia = message.DeteccaoInterferencia;
                currentB975.DeteccaoJammer = message.DeteccaoJammer;
                currentB975.NumeroViolacao = message.NumeroViolacao;
            }

            currentB975.DeviceId = message.DeviceId;
            currentB975.Date = message.OperationDate.Value;
            currentB975.Lqi = message.Lqi;
            currentB975.Radius = deviceLocation.Radius;
            currentB975.Latitude = deviceLocation.Latitude;
            currentB975.Longitude = deviceLocation.Longitude;
            currentB975.LocationCity = GetCityByCoordinates(deviceLocation.Latitude, deviceLocation.Longitude);
        }

        internal B975 GetB975LastPack(string deviceId, string pack)
        {
            if (pack.ToUpper().Equals("A"))
                return _context.B975s.AsNoTracking().OrderByDescending(c => c.Date).FirstOrDefault(c => c.DeviceId == deviceId && c.PackA != null);

            if (pack.ToUpper().Equals("B"))
                return _context.B975s.AsNoTracking().OrderByDescending(c => c.Date).FirstOrDefault(c => c.DeviceId == deviceId && c.PackB != null);

            return _context.B975s.AsNoTracking().OrderByDescending(c => c.Date).FirstOrDefault(c => c.DeviceId == deviceId && c.PackC != null);
        }

        public B975 GetLastB975ByDevice(string deviceId, bool filtrarBloqueios)
        {
            // if (filtrarBloqueios)
            //     return _context.B975s.AsNoTracking().OrderByDescending(c => c.Date).FirstOrDefault(c => c.DeviceId == deviceId && c.Bloqueio);

            return _context.B975s.AsNoTracking().OrderByDescending(c => c.Date).FirstOrDefault(c => c.DeviceId == deviceId);
        }

        internal bool VerifyExistDataB975(string deviceId, string pack, string data, long time)
        {
            if (pack.ToUpper().Equals("A"))
                return _context.B975s.AsNoTracking().OrderByDescending(c => c.Date).Any(c => c.DeviceId == deviceId && c.PackA == data && c.TimeA == time);

            if (pack.ToUpper().Equals("B"))
                return _context.B975s.AsNoTracking().OrderByDescending(c => c.Date).Any(c => c.DeviceId == deviceId && c.PackB == data && c.TimeB == time);

            return _context.B975s.AsNoTracking().OrderByDescending(c => c.Date).Any(c => c.DeviceId == deviceId && c.PackC == data && c.TimeC == time);
        }

        internal B975[] GetB975(string deviceId, string pack)
        {
            if (pack.ToUpper().Equals("A"))
                return _context.B975s.AsNoTracking().OrderByDescending(c => c.Date).Where(c => c.DeviceId == deviceId && c.PackA == null).ToArray();

            if (pack.ToUpper().Equals("B"))
                return _context.B975s.AsNoTracking().OrderByDescending(c => c.Date).Where(c => c.DeviceId == deviceId && c.PackB == null).ToArray();

            return _context.B975s.AsNoTracking().OrderByDescending(c => c.Date).Where(c => c.DeviceId == deviceId && c.PackC == null).ToArray();
        }

        internal async Task SaveB975(B975 b975)
        {
            try
            {
                _context.B975s.Add(b975);
                await _context.SaveChangesAsync();
                // _log.Log("B975 criado.");
            }
            catch (System.Exception ex)
            {
                _log.Log("Sigfox.Save.B975: Error save.", ex.Message, true);
            }
        }

        internal async Task UpdateB975(B975 b975)
        {
            try
            {
                 _context.Entry<Models.B975>(b975).State = EntityState.Modified;
                 await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                _log.Log("Sigfox.Update.B975: Error update.", ex.Message, true);
            }
        }

        // simulation
        public B975 SimulateProcessDataB975(Models.Message message, Models.DeviceLocation deviceLocation)
        {
            B975 currentB975 = new B975();

            string letterPack = message.TypePackage.Substring(1,1);
            BindPack(letterPack, message, currentB975, deviceLocation);

            return currentB975;
        }


        public Message[] ProcessPunctualOldDataB975()
        {
            var messages = _context.Messages.Where(c => c.WasProcessed == false && c.DeviceId != "283B860" &&
                (c.Data.Substring(0, 2).ToUpper().Equals("1A") ||
                c.Data.Substring(0, 2).ToUpper().Equals("1B") ||
                c.Data.Substring(0, 2).ToUpper().Equals("1C"))
                ).ToArray();


            foreach (var m in messages)
            {
                if (VerifyExistDataB975(m.DeviceId, m.TypePackage.Substring(1,1), m.Data, m.Time))
                {
                    UpdateMessageProcessed(m);
                    continue;
                }
                
                Models.DeviceLocation deviceLocation = new Models.DeviceLocation
                {
                    DeviceLocationUId = Guid.NewGuid().ToString(),
                    DeviceId = m.DeviceId,
                    Data = m.Data,
                    Time = m.Time,
                    Latitude = 0,
                    Longitude = 0,
                    Radius = null
                };

                // rodando localhost, tem que ajustar a hora que vem UTC
                m.OperationDate.Value.AddHours(-3);
                ProcessDataB975(m, deviceLocation).Wait();

                UpdateMessageProcessed(m);
            }

            // UpdateMessagesInBlock(messages);

            return messages;

        }

    }
}