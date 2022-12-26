using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using SmartGeoIot.ViewModels;
using System.Linq;
using SmartGeoIot.Models;

namespace SmartGeoIot.Api
{
    [Route("api/[controller]")]
    public class SGIReportController : Controller
    {
        protected readonly SmartGeoIot.Services.RadiodadosService _sgiService;
        public SGIReportController(SmartGeoIot.Services.RadiodadosService sgiService)
        {
            _sgiService = sgiService;
        }

        [HttpGet("{id}")]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-REPORT.READ")]
        public IEnumerable<DashboardViewModels> Get(string id, string de = null, string ate = null, [FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string orderBy = null, [FromQuery] bool blocked = false, [FromQuery] ReportResilType reportType = ReportResilType.Analitico)
        {
            if(id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            var deviceRegistration = _sgiService.GetDeviceRegistrationFull(id);
            var totalCount = new OptionalOutTotalCount();
            var reports = _sgiService.GetReports(id, User, deviceRegistration.Package.Type, skip, top, de, ate, totalCount, orderBy, blocked, false, reportType).ToArray();
            Request.SetListTotalCount(totalCount.Value);

            return reports;
        }

        [HttpGet("download")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-REPORT.READ")]
        public FileResult Download([FromQuery] string id, [FromQuery] string de, [FromQuery] string ate, [FromQuery] int top = 0, [FromQuery] bool blocked = false, [FromQuery] ReportResilType reportType = ReportResilType.Analitico)
        {
            var deviceRegistration = _sgiService.GetDeviceRegistrationFull(id);
            var reports = _sgiService.GetReports(id, User, deviceRegistration.Package.Type, 0, top, de, ate, null, null, blocked, false, reportType).ToArray();

            var excel = new SmartGeoIot.Services.ExcelUtils();
            byte[] excelBytes = null;

            if (deviceRegistration.Package.Type.Equals("10"))
                excelBytes = excel.ExportReports(reports, id, de, ate);
            else if (deviceRegistration.Package.Type.Equals("12"))
                excelBytes = excel.ExportReportsDJRF(reports, id, de, ate);
            else if (deviceRegistration.Package.Type.Equals("81"))
                excelBytes = excel.ExportReportsPQA(reports, id, de, ate);
            else if (deviceRegistration.Package.Type.Equals("21"))
                excelBytes = excel.ExportReportsTRM(reports, id, de, ate);
            else if (deviceRegistration.Package.Type.Equals("23"))
                excelBytes = excel.ExportReportsTRM10(reports, id, de, ate, reportType);
            else if (deviceRegistration.Package.Type.Equals("83"))
                excelBytes = excel.ExportReportsTSP(reports, id, de, ate);
            else
                throw new Box.Common.BoxLogicException("Erro na exportação. Por favor, entre em contato com o suporte.");

            if (excelBytes is null)
                throw new Box.Common.BoxLogicException("Não existem dados para exportação.");

            var report = new FileContentResult(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            report.FileDownloadName = "relatório.xlsx";

            return report;
        }


        [HttpGet("B987/{id}")]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-REPORT.READ")]
        public IEnumerable<MCond> GetReportB987(string id, string de = null, string ate = null, [FromQuery] int skip = 0, [FromQuery] int top = 0)
        {
            if(id == null)
                throw new Box.Common.BoxLogicException("É necessário informar o id do dispositivo.");

            var totalCount = new OptionalOutTotalCount();
            var reports = _sgiService.GetReportMcond(id, skip, top, de, ate, totalCount).ToArray();
            Request.SetListTotalCount(totalCount.Value);

            return reports;
        }

        [HttpGet("B987/download")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-REPORT.READ")]
        public FileResult DownloadMCond([FromQuery] string id, [FromQuery] string de, [FromQuery] string ate, [FromQuery] int top = 0)
        {
            var reports = _sgiService.GetReportMcond(id, 0, top, de, ate).ToArray();

            var excel = new SmartGeoIot.Services.ExcelUtils();
            byte[] excelBytes = null;

            excelBytes = excel.ExportReportsMCond(reports, id, de, ate);

            if (excelBytes is null)
                throw new Box.Common.BoxLogicException("Não existem dados para exportação.");

            var report = new FileContentResult(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            report.FileDownloadName = "relatório-mcond.xlsx";

            return report;
        }


    }
}