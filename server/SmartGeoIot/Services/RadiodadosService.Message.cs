using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Box.Common.Web;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.Extensions;
using SmartGeoIot.Models;
using SmartGeoIot.ViewModels;

namespace SmartGeoIot.Services
{
    public partial class RadiodadosService
    {
        public Message CreateDefaultMessageToCallBack(string deviceId, string pack, long time, bool isLocalHost)
        {
            if (isLocalHost)
            {
                return new Message()
                {
                    Id = Guid.NewGuid().ToString(),
                    DeviceId = deviceId,
                    Data = pack.ToUpper(),
                    Time = time,
                    OperationDate = time.ToString().Length > 10 ? Utils.Timestamp_Milisecodns_ToDateTime_UTC(time) : Utils.TimeStampSecondsToDateTimeUTC(time)
                };
            }
            else
            {
                return new Message()
                {
                    Id = Guid.NewGuid().ToString(),
                    DeviceId = deviceId,
                    Data = pack.ToUpper(),
                    Time = time,
                    OperationDate = time.ToString().Length > 10 ? Utils.Timestamp_ToDateTimeBrasilian(time) : Utils.TimeStampSecondsToDateTime(time)
                };
            }
        } 
    }
}