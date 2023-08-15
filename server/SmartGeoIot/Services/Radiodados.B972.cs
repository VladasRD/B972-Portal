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
        internal void ProcessDataB972(List<Message> messages)
        {
            // _log.Log("Processando dados do projeto B972.", "ProcessDataB972", true);

            List<B972> listB972 = new List<B972>();
            int seconds = 60;
            int miliseconds = 1000;
            int multiplied = 10;
            var projectDevices = GetDevicesByProjectCode(Utils.EnumToAnnotationText(ProjectCode.B972));
            var devices = projectDevices != null ? projectDevices.Select(s => s.DeviceId).ToArray() : null;

            if (devices == null)
                return;

            messages = messages
            .Where(c =>
                c.TypePackage.Equals("84") ||
                c.TypePackage.Equals("85") ||
                c.TypePackage.Equals("86") ||
                c.TypePackage.Equals("87"))
            .ToList();
            messages = messages.Where(c => devices.Any(a => c.DeviceId == a)).OrderBy(o => o.Time).ToList();

            foreach (var message in messages)
            {
                var oldB972 = GetB972(message.DeviceId, message.Time);
                if (oldB972)
                    continue;
                
                var oldB972_85 = GetB972_85(message.DeviceId, message.Time);
                if (oldB972_85)
                    continue;

                if (Convert.ToInt32(message.TypePackage) == (int)PackagesEnum.B972_84)
                {
                    long currentTime = message.Time;

                    listB972.Add(new B972()
                    {
                        DeviceId = message.DeviceId.TrimStart().TrimEnd(),
                        Time = message.Time,
                        Position = 1,
                        Flow = message.VazaoTempo1 * multiplied,
                        Velocity = null,
                        RSSI = null,
                        Source = "SIGFOX",
                        Lqi = message.Lqi,
                        Iq = B972_IQ_Enum.Partial,
                        Date = message.Date
                    });

                    currentTime = (currentTime - (5 * seconds * miliseconds));
                    listB972.Add(new B972()
                    {
                        DeviceId = message.DeviceId.TrimStart().TrimEnd(),
                        Time = currentTime,
                        Position = 2,
                        Flow = message.VazaoTempo2 * multiplied,
                        Velocity = null,
                        RSSI = null,
                        Source = "SIGFOX",
                        Lqi = message.Lqi,
                        Iq = B972_IQ_Enum.Partial,
                        Date = Utils.Timestamp_Milisecodns_ToDateTime_UTC(currentTime)
                    });

                    currentTime = (currentTime - (5 * seconds * miliseconds));
                    listB972.Add(new B972()
                    {
                        DeviceId = message.DeviceId.TrimStart().TrimEnd(),
                        Time = currentTime,
                        Position = 3,
                        Flow = message.VazaoTempo3 * multiplied,
                        Velocity = null,
                        RSSI = null,
                        Source = "SIGFOX",
                        Lqi = message.Lqi,
                        Iq = B972_IQ_Enum.Partial,
                        Date = Utils.Timestamp_Milisecodns_ToDateTime_UTC(currentTime)
                    });

                    currentTime = (currentTime - (5 * seconds * miliseconds));
                    listB972.Add(new B972()
                    {
                        DeviceId = message.DeviceId.TrimStart().TrimEnd(),
                        Time = currentTime,
                        Position = 4,
                        Flow = message.VazaoTempo4 * multiplied,
                        Velocity = null,
                        RSSI = null,
                        Source = "SIGFOX",
                        Lqi = message.Lqi,
                        Iq = B972_IQ_Enum.Partial,
                        Date = Utils.Timestamp_Milisecodns_ToDateTime_UTC(currentTime)
                    });

                    currentTime = (currentTime - (5 * seconds * miliseconds));
                    listB972.Add(new B972()
                    {
                        DeviceId = message.DeviceId.TrimStart().TrimEnd(),
                        Time = currentTime,
                        Position = 5,
                        Flow = message.VazaoTempo5 * multiplied,
                        Velocity = null,
                        RSSI = null,
                        Source = "SIGFOX",
                        Lqi = message.Lqi,
                        Iq = B972_IQ_Enum.Partial,
                        Date = Utils.Timestamp_Milisecodns_ToDateTime_UTC(currentTime)
                    });

                    SaveB972(listB972);
                    listB972 = new List<B972>();
                }

                if (Convert.ToInt32(message.TypePackage) == (int)PackagesEnum.B972_85)
                {
                    var b972_p1 = GetLastB972(message.DeviceId, 1);
                    if (b972_p1 == null)
                        continue;

                    long currentTime = message.Time;
                    b972_p1.Velocity = message.VelocidadeTempo1 * multiplied;
                    b972_p1.VelocityTime = currentTime;
                    b972_p1.Iq = B972_IQ_Enum.Complete;
                    // listB972.Add(b972_p1);
                    UpdateB972(b972_p1);



                    var b972_p2= GetLastB972(message.DeviceId, 2);
                    if (b972_p2 == null)
                        continue;
                    
                    currentTime = (currentTime - (5 * seconds * miliseconds));
                    b972_p2.Velocity = message.VelocidadeTempo2 * multiplied;
                    b972_p2.VelocityTime = currentTime;
                    b972_p2.Iq = B972_IQ_Enum.Complete;
                    // listB972.Add(b972_p2);
                    UpdateB972(b972_p2);



                    var b972_p3 = GetLastB972(message.DeviceId, 3);
                    if (b972_p3 == null)
                        continue;
                    
                    currentTime = (currentTime - (5 * seconds * miliseconds));
                    b972_p3.Velocity = message.VelocidadeTempo3 * multiplied;
                    b972_p3.VelocityTime = currentTime;
                    b972_p3.Iq = B972_IQ_Enum.Complete;
                    // listB972.Add(b972_p3);
                    UpdateB972(b972_p3);



                    var b972_p4 = GetLastB972(message.DeviceId, 4);
                    if (b972_p4 == null)
                        continue;
                    
                    currentTime = (currentTime - (5 * seconds * miliseconds));
                    b972_p4.Velocity = message.VelocidadeTempo4 * multiplied;
                    b972_p4.VelocityTime = currentTime;
                    b972_p4.Iq = B972_IQ_Enum.Complete;
                    // listB972.Add(b972_p4);
                    UpdateB972(b972_p4);




                    var b972_p5 = GetLastB972(message.DeviceId, 5);
                    if (b972_p5 == null)
                        continue;
                    
                    currentTime = (currentTime - (5 * seconds * miliseconds));
                    b972_p5.Velocity = message.VelocidadeTempo5 * multiplied;
                    b972_p5.VelocityTime = currentTime;
                    b972_p5.Iq = B972_IQ_Enum.Complete;
                    // listB972.Add(b972_p5);
                    UpdateB972(b972_p5);
                }

                if (Convert.ToInt32(message.TypePackage) == (int)PackagesEnum.B972_86)
                {
                    var _total = Utils.FromFloatSafe(message.Total_P982U4);
                    var _parcial = Utils.FromFloatSafe(message.Parcial_P982U4);

                    UpdateB972_P982U4_86(message.DeviceId, (decimal)_total, (decimal)_parcial, message.Temperatura_P982U4, message.Time);
                }

                if (Convert.ToInt32(message.TypePackage) == (int)PackagesEnum.B972_87)
                {
                    UpdateB972_P982U4_87(message.DeviceId, message.Flags_P982U4, message.Quality_P982U4, message.Time);
                }
            }

            // _log.Log("Finalizando dados do projeto B972.", "ProcessDataB972", true);
        }

        internal void SaveB972(List<B972> b972)
        {
            if (b972.Count > 0)
            {
                foreach (var item in b972)
                {
                    bool oldB972 = GetB972(item.DeviceId, item.Time);
                    if (!oldB972)
                    {
                        _context.B972s.Add(item);
                        _context.SaveChanges();
                        _log.Log("B972 criado.");
                    }
                }
            }
        }

        internal void UpdateB972(B972 b972)
        {
            try
            {
                 _context.Entry<Models.B972>(b972).State = EntityState.Modified;
                 _context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                _log.Log("Sigfox.UpdateB972: Error update.", ex.Message, true);
            }
        }

        internal void UpdateListB972(List<B972> b972)
        {
            try
            {     
                if (b972.Count > 0)
                {
                    _context.UpdateRange(b972);
                    _context.SaveChanges();
                }
            }
            catch (System.Exception ex)
            {
                _log.Log("Sigfox.UpdateB972: Error update range.", ex.Message, true);
            }
        }

        internal void UpdateB972_P982U4_86(string deviceId, decimal total, decimal partial, decimal temperature, long time)
        {
            var listB972 = _context.B972s.Where(c => c.DeviceId == deviceId && (c.Total == null || c.Partial == null || c.Temperature == null)).ToArray();
            foreach (var item in listB972)
            {
                try
                {     
                    item.Total = total;
                    item.Partial = partial;
                    item.Temperature = temperature/10;
                    item.Pack86Time = time;
                    
                    _context.Entry<Models.B972>(item).State = EntityState.Detached;
                    _context.Entry<Models.B972>(item).State = EntityState.Modified;

                    // _context.Entry(oldRole).State = EntityState.Modified;
                    // _context.Entry(item).CurrentValues.SetValues(item);

                    // _context.Update(item);
                    _context.SaveChanges();

                    // _context.B972s.Attach(item);
                    // var entry = _context.Entry(item);
                    // entry.State = EntityState.Modified;
                    // _context.SaveChanges();
                }
                catch (System.Exception ex)
                {
                    _log.Log("Sigfox.UpdateB972: Error update range.", ex.Message, true);
                }
            }

            // try
            // {
            //      _context.UpdateRange(listB972);
            //      _context.SaveChanges();
            // }
            // catch (System.Exception ex)
            // {
            //     _log.Log("Sigfox.UpdateB972: Error update range.", ex.Message, true);
            // }
        }

        internal void UpdateB972_P982U4_87(string deviceId, string flags, decimal quality, long time)
        {
            var listB972 = _context.B972s.Where(c => c.DeviceId == deviceId && (c.Flags == null || c.Quality == null)).ToArray();
            foreach (var item in listB972)
            {
                try
                {     
                    item.Flags = flags;
                    item.Quality = quality;
                    item.Pack87Time = time;

                    _context.Entry<Models.B972>(item).State = EntityState.Detached;
                    _context.Entry<Models.B972>(item).State = EntityState.Modified;

                    // _context.Update(item);
                    _context.SaveChanges();

                    // _context.B972s.Attach(item);
                    // var entry = _context.Entry(item);
                    // entry.State = EntityState.Modified;
                    // _context.SaveChanges();

                }
                catch (System.Exception ex)
                {
                    _log.Log("Sigfox.UpdateB972: Error update range.", ex.Message, true);
                }
            }

            // try
            // {
            //      _context.UpdateRange(listB972);
            //      _context.SaveChanges();
            // }
            // catch (System.Exception ex)
            // {
            //     _log.Log("Sigfox.UpdateB972: Error update range.", ex.Message, true);
            // }
        }

        internal B972 GetLastB972(string deviceId, int position)
        {
            return _context.B972s.OrderByDescending(o => o.Time).FirstOrDefault(c => c.DeviceId == deviceId && c.Position == position);
        }

        internal bool GetB972(string deviceId, long time)
        {
            return _context.B972s.AsNoTracking().Any(c => c.DeviceId == deviceId && c.Time == time);
        }

        internal bool GetB972_85(string deviceId, long time)
        {
            return _context.B972s.AsNoTracking().Any(c => c.DeviceId == deviceId && c.VelocityTime == time);
        }
        
        
    }
}