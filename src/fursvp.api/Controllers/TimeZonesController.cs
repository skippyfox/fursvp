using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneNames;

namespace fursvp.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeZonesController
    {
        private ILogger<EventController> _logger { get; }

        public TimeZonesController(ILogger<EventController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetTimeZones()
        {
            //TZNames.GetDisplayNames("en-US", true);
            //TODO: Use GetDisplayNames and TimeZoneConverter (if necessary) and join with system ids below

            var results = TimeZoneInfo.GetSystemTimeZones().Select(x => new
            {
                x.Id,
                x.DisplayName
            });

            return new JsonResult(results);
        }
    }
}
