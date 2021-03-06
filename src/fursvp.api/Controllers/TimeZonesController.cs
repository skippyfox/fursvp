﻿// <copyright file="TimeZonesController.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Controllers
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Web Api Controller used to get Time Zone details.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TimeZonesController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZonesController"/> class.
        /// </summary>
        public TimeZonesController()
        {
        }

        /// <summary>
        /// Gets a list of all Time Zones by their unique Id and DisplayName.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> for the web response.</returns>
        [HttpGet]
        public IActionResult GetTimeZones()
        {
            var results = TimeZoneInfo.GetSystemTimeZones().Select(x => new
            {
                x.Id,
                x.DisplayName,
            });

            return new JsonResult(results);
        }
    }
}
