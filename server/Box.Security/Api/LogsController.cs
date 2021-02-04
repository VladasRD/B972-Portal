using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Box.Common;
using Box.Common.Web;
using Box.Security.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Box.Security.Api
{
    [Route("api/[controller]")]
    public class LogsController : Controller
    {
        private readonly Services.LogService _logService;

        public LogsController(Services.LogService logService)
        {
            _logService = logService;
        }

        [HttpGet]        
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "LOG.READ")]
        public IEnumerable<Log> Get([FromQuery] string filter, DateTime? fromDate = null, DateTime? toDate = null, [FromQuery] int skip = 0, [FromQuery] int top = 0)
        {
            var totalCount = new OptionalOutTotalCount();

            var logs = _logService.GetLogs(filter, skip, top, fromDate, toDate, totalCount).ToList();
            Request.SetListTotalCount(totalCount.Value);

            return logs;
        }

        [HttpGet("{id}")]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "LOG.READ")]
        public Log Get(Guid id)
        {            
            return _logService.GetLog(id);
        }
    }
}
