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
        // public List<B982_S> GetVazaosPack84(string dataPack84, long time, string deviceId)
        // {
        //      Message currentDataDevice = _context.Messages
        //         .OrderByDescending(o => o.Time)
        //         .FirstOrDefault(f => f.DeviceId == deviceId && f.TypePackage.Equals("83"));
            
        //     List<B982_S> list = new List<B982_S>();
        //     int seconds = 60;
        //     int miliseconds = 1000;

        //     long currentTime = 0;
        //     currentTime = time;

        //     var (_data, _operator) = GeneratePack83(currentDataDevice.Data, dataPack84, 0, 4);
        //     list.Add(new B982_S()
        //     {
        //         DeviceId = deviceId,
        //         Time = currentTime,
        //         OriginPack = dataPack84,
        //         RolloverCounter = currentDataDevice.RolloverCounter,
        //         SeqNumber = currentDataDevice.SeqNumber+1,
        //         NbFrames = currentDataDevice.NbFrames,
        //         Operator = _operator,
        //         Country = currentDataDevice.Country,
        //         Lqi = currentDataDevice.Lqi,
        //         OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
        //     });
            
        //     currentTime = time - (3 * seconds * miliseconds);
        //     var (_data2, _operator2) = GeneratePack83(currentDataDevice.Data, dataPack84, 4, 4);
        //     list.Add(new B982_S()
        //     {
        //         Id = currentTime.ToString(),
        //         DeviceId = deviceId,
        //         Time = currentTime,
        //         Data = _data2,
        //         RolloverCounter = currentDataDevice.RolloverCounter,
        //         SeqNumber = currentDataDevice.SeqNumber+2,
        //         NbFrames = currentDataDevice.NbFrames,
        //         Operator = _operator2,
        //         Country = currentDataDevice.Country,
        //         Lqi = currentDataDevice.Lqi,
        //         OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
        //     });
            
        //     currentTime = time - (6 * seconds * miliseconds);
        //     var (_data3, _operator3) = GeneratePack83(currentDataDevice.Data, dataPack84, 8, 4);
        //     list.Add(new B982_S()
        //     {
        //         Id = currentTime.ToString(),
        //         DeviceId = deviceId,
        //         Time = currentTime,
        //         Data = _data3,
        //         RolloverCounter = currentDataDevice.RolloverCounter,
        //         SeqNumber = currentDataDevice.SeqNumber+3,
        //         NbFrames = currentDataDevice.NbFrames,
        //         Operator = _operator3,
        //         Country = currentDataDevice.Country,
        //         Lqi = currentDataDevice.Lqi,
        //         OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
        //     });
            
        //     currentTime = time - (9 * seconds * miliseconds);
        //     var (_data4, _operator4) = GeneratePack83(currentDataDevice.Data, dataPack84, 12, 4);
        //     list.Add(new B982_S()
        //     {
        //         Id = currentTime.ToString(),
        //         DeviceId = deviceId,
        //         Time = currentTime,
        //         Data = _data4,
        //         RolloverCounter = currentDataDevice.RolloverCounter,
        //         SeqNumber = currentDataDevice.SeqNumber+4,
        //         NbFrames = currentDataDevice.NbFrames,
        //         Operator = _operator4,
        //         Country = currentDataDevice.Country,
        //         Lqi = currentDataDevice.Lqi,
        //         OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
        //     });
            
        //     currentTime = time - (12 * seconds * miliseconds);
        //     var (_data5, _operator5) = GeneratePack83(currentDataDevice.Data, dataPack84, 16, 4);
        //     list.Add(new B982_S()
        //     {
        //         Id = currentTime.ToString(),
        //         DeviceId = deviceId,
        //         Time = currentTime,
        //         Data = _data5,
        //         RolloverCounter = currentDataDevice.RolloverCounter,
        //         SeqNumber = currentDataDevice.SeqNumber+5,
        //         NbFrames = currentDataDevice.NbFrames,
        //         Operator = _operator5,
        //         Country = currentDataDevice.Country,
        //         Lqi = currentDataDevice.Lqi,
        //         OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
        //     });
            
        //     return list;
        // }





        public List<Message> GetVazaosPack84(string dataPack84, long time, string deviceId)
        {
             Message currentDataDevice = _context.Messages
                .OrderByDescending(o => o.Time)
                .FirstOrDefault(f => f.DeviceId == deviceId && f.TypePackage.Equals("83"));
            
            List<Message> list = new List<Message>();
            int seconds = 60;
            int miliseconds = 1;

            long currentTime = 0;
            currentTime = time;

            var (_data, _operator) = GeneratePack83(currentDataDevice.Data, dataPack84, 0, 4);
            list.Add(new Message()
            {
                Id = Utils.TimeZerosForRight(currentTime.ToString(), 13).ToString(),
                DeviceId = deviceId,
                Time = Utils.TimeZerosForRight(currentTime.ToString(), 13),
                Data = _data,
                RolloverCounter = currentDataDevice.RolloverCounter,
                SeqNumber = currentDataDevice.SeqNumber+1,
                NbFrames = currentDataDevice.NbFrames,
                Operator = _operator,
                Country = currentDataDevice.Country,
                Lqi = currentDataDevice.Lqi,
                OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
            });
            
            currentTime = time - (3 * seconds * miliseconds);
            var (_data2, _operator2) = GeneratePack83(currentDataDevice.Data, dataPack84, 4, 4);
            list.Add(new Message()
            {
                Id = Utils.TimeZerosForRight(currentTime.ToString(), 13).ToString(),
                DeviceId = deviceId,
                Time = Utils.TimeZerosForRight(currentTime.ToString(), 13),
                Data = _data2,
                RolloverCounter = currentDataDevice.RolloverCounter,
                SeqNumber = currentDataDevice.SeqNumber+2,
                NbFrames = currentDataDevice.NbFrames,
                Operator = _operator2,
                Country = currentDataDevice.Country,
                Lqi = currentDataDevice.Lqi,
                OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
            });
            
            currentTime = time - (6 * seconds * miliseconds);
            var (_data3, _operator3) = GeneratePack83(currentDataDevice.Data, dataPack84, 8, 4);
            list.Add(new Message()
            {
                Id = Utils.TimeZerosForRight(currentTime.ToString(), 13).ToString(),
                DeviceId = deviceId,
                Time = Utils.TimeZerosForRight(currentTime.ToString(), 13),
                Data = _data3,
                RolloverCounter = currentDataDevice.RolloverCounter,
                SeqNumber = currentDataDevice.SeqNumber+3,
                NbFrames = currentDataDevice.NbFrames,
                Operator = _operator3,
                Country = currentDataDevice.Country,
                Lqi = currentDataDevice.Lqi,
                OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
            });
            
            currentTime = time - (9 * seconds * miliseconds);
            var (_data4, _operator4) = GeneratePack83(currentDataDevice.Data, dataPack84, 12, 4);
            list.Add(new Message()
            {
                Id = Utils.TimeZerosForRight(currentTime.ToString(), 13).ToString(),
                DeviceId = deviceId,
                Time = Utils.TimeZerosForRight(currentTime.ToString(), 13),
                Data = _data4,
                RolloverCounter = currentDataDevice.RolloverCounter,
                SeqNumber = currentDataDevice.SeqNumber+4,
                NbFrames = currentDataDevice.NbFrames,
                Operator = _operator4,
                Country = currentDataDevice.Country,
                Lqi = currentDataDevice.Lqi,
                OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
            });
            
            currentTime = time - (12 * seconds * miliseconds);
            var (_data5, _operator5) = GeneratePack83(currentDataDevice.Data, dataPack84, 16, 4);
            list.Add(new Message()
            {
                Id = Utils.TimeZerosForRight(currentTime.ToString(), 13).ToString(),
                DeviceId = deviceId,
                Time = Utils.TimeZerosForRight(currentTime.ToString(), 13),
                Data = _data5,
                RolloverCounter = currentDataDevice.RolloverCounter,
                SeqNumber = currentDataDevice.SeqNumber+5,
                NbFrames = currentDataDevice.NbFrames,
                Operator = _operator5,
                Country = currentDataDevice.Country,
                Lqi = currentDataDevice.Lqi,
                OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
            });
            
            return list;
        }






        public void CreateB982_SByData84(List<B982_S> b982_S)
        {
            // _context.Entry<B982_S>(b982_S).State = EntityState.Added;
            _context.B982_S.AddRange(b982_S);
            _context.SaveChanges(true);
            // _log.Log($"Criação do pacote B982_S: {b982_S.OriginPack} com base no 84.");
        }



        // public void CreateB982_SByData84List(List<Message> messages, string originPack)
        // {
        //     foreach (var message in messages)
        //     {
        //         decimal _flow = 0;
        //         decimal _total = 0;
        //         decimal _partial = 0;
        //         string _calha = string.Empty;
        //         string _calhaAlerta = string.Empty;

        //         if (messages.Operator != null)
        //         {
        //             _flow = messages.Operator;
        //             dashboard.Date = Utils.TimeStampSecondsToDateTimeUTC(messages.Time);
        //         }
        //         else
        //         {
        //             _flow = Utils.FromFloatSafe(messages.Vazao);
        //             // dashboard.Vazao = String.Format("{0:0.000}", _vazao);
        //         }

        //         var _totalizacao = Utils.FromFloatSafe(messages.Totalizacao); // total
        //         _total = (decimal)_totalizacao;
        //         // dashboard.Totalizacao = String.Format("{0:0}", _totalizacao);
        //         _calha = messages.Calha;
        //         _calhaAlerta = messages.CalhaAlerta;


        //         _partial = (decimal)_totalizacao;


        //         B982_S b982_S = new B982_S()
        //         {
        //             DeviceId = message.DeviceId,
        //             Time = message.Time,
        //             OriginPack = originPack,
        //             Flow = _flow,
        //             Total = _total,
        //             Partial = _partial,
        //             Calha = _calha,
        //             CalhaAlerta = _calhaAlerta,
        //             RSSI= message.
        //         };

        //         _context.Entry<B982_S>(b982_S).State = EntityState.Added;
        //         _context.SaveChanges(true);
        //     }
            
        // }


        
        public async Task<bool> ExistPackProcessB982_S(long time, string deviceId)
        {
            return await _context.B982_S.AnyAsync(c => c.DeviceId == deviceId && c.Time == time);
        }
        
    }
}