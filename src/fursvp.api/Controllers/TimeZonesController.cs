// <copyright file="TimeZonesController.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Controllers
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("api/[controller]")]
    public class TimeZonesController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZonesController"/> class.
        /// </summary>
        /// <param name="logger">The application event logger.</param>
        public TimeZonesController(ILogger<EventController> logger)
        {
            this.Logger = logger;
        }

        private ILogger<EventController> Logger { get; }

        [HttpGet]
        public IActionResult GetTimeZones()
        {
            //// TZNames.GetDisplayNames("en-US", true);

            // TODO: Use GetDisplayNames and TimeZoneConverter (if necessary) and join with system ids below
            var results = TimeZoneInfo.GetSystemTimeZones().Select(x => new
            {
                x.Id,
                x.DisplayName,
            });

            return new JsonResult(results);
        }
    }
}
