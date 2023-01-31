using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Box.Common.Web;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.Extensions;
using SmartGeoIot.Models;

namespace SmartGeoIot.Services
{
    public partial class RadiodadosService
    {
        internal async Task VerifyAndAlertAlarmsB987(MCond currentData)
        {
            try
            {
                bool canNotify = false;
                var deviceFather = GetDeviceFather(currentData.DeviceId);
                var lastData = _context.MConds.LastOrDefault(c => c.DeviceId == deviceFather && c.Id != currentData.Id);
                if (lastData == null)
                    return;

                // if (currentData.SupAlarmLevelMax != lastData.SupAlarmLevelMax || currentData.SupAlarmLevelMin != lastData.SupAlarmLevelMin ||
                // currentData.SupStateBomb != lastData.SupStateBomb || currentData.PortFireAlarm != lastData.PortFireAlarm ||
                // currentData.PortFireState != lastData.PortFireState || currentData.PortIvaAlarm != lastData.PortIvaAlarm || currentData.PortIvaState != lastData.PortIvaState)
                // {
                //     canNotify = true;
                // }
                if (currentData.SupAlarmLevelMax != lastData.SupAlarmLevelMax || currentData.SupAlarmLevelMin != lastData.SupAlarmLevelMin ||
                currentData.SupStateBomb != lastData.SupStateBomb || currentData.PortFireState != lastData.PortFireState || currentData.PortIvaState != lastData.PortIvaState)
                {
                    canNotify = true;
                }

                if (canNotify)
                {
                    List<string> emailsNotify = new List<string>();
                    List<string> phoneNumbers = new List<string>();
                    var clients = GetClientsByDevice(deviceFather);
                    var admEmails = _SMTPsettings.MockRecipientCopy.Split(";").ToList();

                    emailsNotify = admEmails;
                    phoneNumbers = _sgiSettings.ADM_PHONE.Split(";").ToList();
                    phoneNumbers.Add(_sgiSettings.TECHNICAL_PHONE);

                    if (clients != null)
                    {
                        if (clients.Count() > 0)
                        {
                            var clientMails = clients.Where(c => c.EmailNotification).Select(s => s.Email).ToList();
                            foreach (var item in clientMails)
                            {
                                emailsNotify.Add(item);
                            }

                            var clientPhones = clients.Where(c => c.WhatsAppNotification).Select(s => s.Phone).ToList();
                            foreach (var item in clientPhones)
                            {
                                phoneNumbers.Add($"+55{item}");
                            }
                        }
                    }
                    
                    await NotifyStateAlertsChangedDeviceB987(emailsNotify.ToArray(), currentData, lastData, deviceFather, phoneNumbers.ToArray());
                }
            }
            catch (System.Exception ex)
            {
                _log.Log("VerifyAndAlertAlarmsB987: Erro ao verificar alarmes do projeto B987.", ex.Message);
            }
        }

        internal MCond ProcessDataB987(List<Message> messages)
        {
            _log.Log("Processando dados do projeto B987.", "ProcessDataB987", true);
            MCond _result = null;

            foreach (var message in messages)
            {
                try
                {
                    var deviceFather = GetDeviceFather(message.DeviceId);
                    McondType deviceType = GetMCondType(message);
                    var dataB987 = GetDataB987(deviceFather);

                    DateTime _today = DateTime.Today;
                    bool _stateBomb = false;
                    bool _alarmLevelMin = false;
                    bool _alarmLevelMax = false;
                    bool _portFireAlarm = false;
                    bool _portIvaAlarm = false;
                    bool _portFireState = false;
                    bool _portIvaState = false;
                    
                    (_alarmLevelMin, _alarmLevelMax) = message.MCond_NivelBaixoAlto;
                    double _level = Utils.FromFloatSafe(message.MCond_Nivel);
                    
                    if (McondType.Superior == deviceType)
                        _stateBomb = message.MCond_BombaHidraulica;
                    
                    if (McondType.Portaria == deviceType)
                        (_portFireAlarm, _portFireState, _portIvaState, _portIvaAlarm) = message.MCond_AlertaIncendioIVA;

                    if (dataB987 == null)
                    {
                        // criar novo registro (superior ou inferior ou portaria)
                        _result = CreateMCOnd(deviceFather, deviceType, message, _alarmLevelMin, _alarmLevelMax, _stateBomb, _level, _portFireAlarm, _portIvaAlarm, _portFireState, _portIvaState);
                    }
                    else
                    {
                        // verificar o tipo de sensor e atualizar registro (superior ou inferior ou portaria)
                        if (McondType.Superior == deviceType)
                        {
                            // se já existe, vai para o próximo registro
                            // if (dataB987.PackSup != null)
                            // {
                            //     if (!VerifyExistPack(deviceFather, deviceType, message.Data))
                            _result = CreateMCOnd(deviceFather, deviceType, message, _alarmLevelMin, _alarmLevelMax, _stateBomb, _level, _portFireAlarm, _portIvaAlarm, _portFireState, _portIvaState);
                            //     continue;
                            // }
                            
                            // se não existe, atualiza o registro existente com o pacote
                            dataB987.PackSup = message.Data;
                            dataB987.SupStateBomb = _stateBomb;
                            dataB987.SupAlarmLevelMin = _alarmLevelMin;
                            dataB987.SupAlarmLevelMax = _alarmLevelMax;
                            dataB987.SupLevel= _level;
                        }

                        if (McondType.Inferior == deviceType)
                        {
                            // se já existe, vai para o próximo registro
                            // if (dataB987.PackInf != null)
                            // {
                            //     if (!VerifyExistPack(deviceFather, deviceType, message.Data))
                            _result = CreateMCOnd(deviceFather, deviceType, message, _alarmLevelMin, _alarmLevelMax, _stateBomb, _level, _portFireAlarm, _portIvaAlarm, _portFireState, _portIvaState);
                            //     continue;
                            // }
                            
                            // se não existe, atualiza o registro existente com o pacote
                            dataB987.PackInf = message.Data;
                            dataB987.InfAlarmLevelMin = _alarmLevelMin;
                            dataB987.InfAlarmLevelMax = _alarmLevelMax;
                            dataB987.InfLevel = _level;
                        }

                        if (McondType.Portaria == deviceType)
                        {
                            // se já existe, vai para o próximo registro
                            // if (dataB987.PackPort != null)
                            // {
                            //     if (!VerifyExistPack(deviceFather, deviceType, message.Data))
                            _result = CreateMCOnd(deviceFather, deviceType, message, _alarmLevelMin, _alarmLevelMax, _stateBomb, _level, _portFireAlarm, _portIvaAlarm, _portFireState, _portIvaState);
                            //     continue;
                            // }
                            
                            // se não existe, atualiza o registro existente com o pacote
                            dataB987.PackPort = message.Data;
                            dataB987.PortFireAlarm = _portFireAlarm;
                            dataB987.PortIvaAlarm = _portIvaAlarm;

                            dataB987.PortFireState = _portFireState;
                            dataB987.PortIvaState = _portIvaState;
                        }

                        // atualiza registro no banco de dados
                        _result = dataB987;
                        UpdateB987(dataB987);
                    }
                }
                catch (System.Exception ex)
                {
                    _log.Log("SigFox.SigfoxSaveMessages.ProcessDataB987: Erro ao processar dados B987.", ex.Message);
                    continue;
                }
            }

            _log.Log("Finalizando dados do projeto B987.", "ProcessDataB987", true);
            return _result;
        }

        internal MCond GetDataB987(string deviceId)
        {
            return _context.MConds.LastOrDefault(c => c.DeviceId == deviceId);
        }

        internal string GetDeviceFather(string deviceId)
        {
            Device device = _context.Devices.FirstOrDefault(c => c.Id == deviceId);
            return device != null ? device.Name : string.Empty;
        }

        internal McondType GetMCondType(Message messages)
        {
            McondType type= McondType.Portaria;
            try
            {
                double deviceSubtipo = Utils.FromFloatSafe(messages.MCondSubtipo);

                type = (McondType)Convert.ToInt32(deviceSubtipo);

                return type;
            }
            catch (System.Exception)
            {
                return type;
            }
        }

        internal bool VerifyExistPack(string deviceFather, McondType deviceType, string pack)
        {
            bool _result = false;

            if (McondType.Superior == deviceType)
                _result = _context.MConds.Any(c => c.DeviceId == deviceFather && c.PackSup == pack);

            if (McondType.Inferior == deviceType)
                _result = _context.MConds.Any(c => c.DeviceId == deviceFather && c.PackInf == pack);

            if (McondType.Portaria == deviceType)
                _result = _context.MConds.Any(c => c.DeviceId == deviceFather && c.PackPort == pack);

            return _result;
        }

        internal MCond CreateMCOnd(string deviceFather, McondType deviceType, Message message, bool _alarmLevelMin, bool _alarmLevelMax, bool _stateBomb, double _level, bool _portFireAlarm, bool _portIvaAlarm, bool _portFireState, bool _portIvaState)
        {
            DateTime _today = message.OperationDate.HasValue ? message.OperationDate.Value : DateTime.Today;
            MCond mcond = new MCond()
            {
                DeviceId = deviceFather,
                Time = _today.ToFileTime(),
                
                PackInf = (McondType.Inferior == deviceType) ? message.Data : null,
                InfAlarmLevelMin = (McondType.Inferior == deviceType) ? _alarmLevelMin : false,
                InfAlarmLevelMax = (McondType.Inferior == deviceType) ? _alarmLevelMax : false,
                InfLevel = (McondType.Inferior == deviceType) ? _level : 0,

                SupStateBomb = (McondType.Superior == deviceType) ? _stateBomb : false,
                PackSup = (McondType.Superior == deviceType) ? message.Data : null,
                SupAlarmLevelMin = (McondType.Superior == deviceType) ? _alarmLevelMin : false,
                SupAlarmLevelMax = (McondType.Superior == deviceType) ? _alarmLevelMax : false,
                SupLevel= (McondType.Superior == deviceType) ? _level : 0,

                PackPort = (McondType.Portaria == deviceType) ? message.Data : null,
                PortFireAlarm = (McondType.Portaria == deviceType) ? _portFireAlarm : false,
                PortIvaAlarm = (McondType.Portaria == deviceType) ? _portIvaAlarm : false,
                PortFireState = (McondType.Portaria == deviceType) ? _portFireState : false,
                PortIvaState = (McondType.Portaria == deviceType) ? _portIvaState : false,

                Date = _today
            };

            // insere registro no banco de dados
            SaveB987(mcond);
            return mcond;
        }

        internal void SaveB987(MCond mCond)
        {
            _context.MConds.Add(mCond);
            _context.SaveChanges();
        }

        internal void UpdateB987(MCond mCond)
        {
            try
            {
                 _context.Entry<Models.MCond>(mCond).State = EntityState.Modified;
                 _context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                _log.Log("Sigfox.UpdateB987: Error update.", ex.Message, true);
            }
        }

        public IEnumerable<MCond> GetReportMcond(string id, int skip = 0, int top = 0, string de = null, string ate = null, OptionalOutTotalCount totalCount = null)
        {
            var reports = _context.MConds.Where(c => c.DeviceId == id).ToList();

            if (!de.Equals("null"))
            {
                var firstDate = Convert.ToDateTime(de).ToUniversalTime().AddHours(-3);
                reports = reports.Where(c => c.Date >= firstDate).ToList();
            }
            if (!ate.Equals("null"))
            {
                var lastDate = Convert.ToDateTime(ate).ToUniversalTime().AddDays(1).AddHours(-3).AddMinutes(-1);
                reports = reports.Where(c => c.Date <= lastDate).ToList();
            }
            
            if (totalCount != null)
                totalCount.Value = reports.Count();

            reports = reports.OrderBy(o => o.Date).ToList();

            if (skip != 0)
                reports = reports.Skip(skip).ToList();

            if (top != 0)
                reports = reports.Take(top).ToList();

            return reports.ToArray();
        }

    }
}