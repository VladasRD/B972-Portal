using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGeoIot.Models
{
    public class StandardResponse<DataType>
    {
        public string MessageToUser { get; set; } = null;
        public string DebugCode { get; set; } = null;
        public DataType Data { get; set; }
    }
    
    public class StandardPagedResponse<DataType> : StandardResponse<DataType>
    {
        public int PageNumber { get; set; }
        public int ItemsOnThisPage { get; set; }
        public int TotalItensOfRequest { get; set; }
        public int TotalPages { get; set; }
    }
}