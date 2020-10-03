using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InterviewTaskLogAPI.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InterviewTaskLogAPI.Controllers
{
    [ApiController]
    // custom route for clarity
    [Route("[controller]/api")]
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> _logger;

        // adding logic via DI
        private readonly IBusinessLogic _businessLogic;

        public LogController(ILogger<LogController> logger, IBusinessLogic businessLogic)
        {
            _logger = logger;
            _businessLogic = businessLogic;
        }

        // POST log/api
        [HttpPost]
        public ContentResult Post([FromBody] InputLog inputLog)
        {
            try
            {
                // as little logic in the controller as possible
                return Content(_businessLogic.ProcessLog(inputLog).ToString());
            }
            catch
            {
                return Content("false");
            }
            
        }
    }
}
