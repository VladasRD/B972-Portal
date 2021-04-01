using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using SmartGeoIot.ViewModels;
using System.Linq;

namespace SmartGeoIot.Api
{
    [Route("api/[controller]")]
    public class SGIReportController : Controller
    {
        protected readonly SmartGeoIot.Services.SmartGeoIotService _sgiService;
        public SGIReportController(SmartGeoIot.Services.SmartGeoIotService sgiService)
        {
            _sgiService = sgiService;
        }

        [HttpGet("{id}")]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-REPORT.READ")]
        public IEnumerable<DashboardViewModels> Get(string id, string de = null, string ate = null, [FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string orderBy = null, [FromQuery] bool blocked = false)
        {
            if(id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            var deviceRegistration = _sgiService.GetDeviceRegistrationFull(id);
            var totalCount = new OptionalOutTotalCount();
            var reports = _sgiService.GetReports(id, User, deviceRegistration.Package.Type, skip, top, de, ate, totalCount, orderBy, blocked).ToArray();
            Request.SetListTotalCount(totalCount.Value);

            return reports;
        }

        [HttpGet("download")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-REPORT.READ")]
        public FileResult Download([FromQuery] string id, [FromQuery] string de, [FromQuery] string ate, [FromQuery] int top = 0, [FromQuery] bool blocked = false)
        {
            var deviceRegistration = _sgiService.GetDeviceRegistrationFull(id);
            var reports = _sgiService.GetReports(id, User, deviceRegistration.Package.Type, 0, top, de, ate, null, null, blocked).ToArray();

            var excel = new SmartGeoIot.Services.ExcelUtils();
            byte[] excelBytes = null;

            if (deviceRegistration.Package.Type.Equals("10"))
                excelBytes = excel.ExportReports(reports, id, de, ate);

            if (deviceRegistration.Package.Type.Equals("12"))
                excelBytes = excel.ExportReportsDJRF(reports, id, de, ate);

            if (deviceRegistration.Package.Type.Equals("81"))
                excelBytes = excel.ExportReportsPQA(reports, id, de, ate);

            if (deviceRegistration.Package.Type.Equals("21"))
                excelBytes = excel.ExportReportsTRM(reports, id, de, ate);
            
            if (deviceRegistration.Package.Type.Equals("23"))
                excelBytes = excel.ExportReportsTRM10(reports, id, de, ate);

            var report = new FileContentResult(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            report.FileDownloadName = "relatório.xlsx";

            return report;
        }
    }
}