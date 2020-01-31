// <copyright file="AuthController.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Api.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Fursvp.Api.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Authentication controller.
    /// </summary>
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Instantly logs in a tester with the email address provided. For Debugging only.
        /// </summary>
        /// <param name="emailAddress">The email address to log in as.</param>
        /// <returns>The generated token.</returns>
        [ServiceFilter(typeof(DebugModeOnlyFilter))]
        [HttpPost]
        [Route("debug")]
        public IActionResult DebugAuth([FromBody]string emailAddress)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Some secret goes here!"); // TODO
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, emailAddress),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);
            var writtenToken = tokenHandler.WriteToken(createdToken);

            return this.Ok(writtenToken);
        }
    }
}
