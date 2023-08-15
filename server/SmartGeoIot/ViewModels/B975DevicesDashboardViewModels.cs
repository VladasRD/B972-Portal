using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGeoIot.ViewModels
{
    public class B975DevicesDashboardViewModels
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public DateTime? Date { get; set; }
        public string StatusDJ { get; set; }
        public int? ContadorCarencias { get; set; }
        public int? ContadorBloqueios { get; set; }

        [MaxLength(500)]
        public string LocationCity { get; set; }

        [NotMapped]
        public bool HasServiceDeskOpened { get; set; }

        [NotMapped]
        public bool HasHistoryServiceDesk { get; set; }
    }
}