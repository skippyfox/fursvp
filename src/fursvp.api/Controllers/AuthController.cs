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
    using System.Threading.Tasks;
    using Fursvp.Api.Filters;
    using fursvp.api.Requests;
    using Fursvp.Communication;
    using Fursvp.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Authentication controller.
    /// </summary>
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;
        private readonly IEmailer emailer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="memoryCache">The application memory cache.</param>
        /// <param name="emailer">The instance of <see cref="IEmailer"/> used to suppress or send emails.</param>
        public AuthController(IConfiguration configuration, IMemoryCache memoryCache, IEmailer emailer)
        {
            this.configuration = configuration;
            this.memoryCache = memoryCache;
            this.emailer = emailer;
        }

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
            var token = this.CreateVerificationToken(emailAddress);
            return this.Ok(token);
        }

        /// <summary>
        /// Logs in a tester with the email address provided by verifying a cached verification code.
        /// </summary>
        /// <param name="verifyEmailRequest">The email address and verification code.</param>
        /// <returns>OkObjectResult with the generated token on success, UnauthorizedResult otherwise.</returns>
        [HttpPost]
        [Route("verifyemail")]
        public IActionResult VerifyEmail([FromBody]VerifyEmailRequest verifyEmailRequest)
        {
            var verificationCodeCacheKey = VerificationCodeCacheKey(verifyEmailRequest.EmailAddress);

            if (this.memoryCache.TryGetValue(verificationCodeCacheKey, out string verificationCode)
                && verificationCode == verifyEmailRequest.VerificationCode)
            {
                // authentication successful so generate jwt token
                var token = this.CreateVerificationToken(verifyEmailRequest.EmailAddress);
                return this.Ok(token);
            }

            return this.Unauthorized();
        }

        /// <summary>
        /// Sends a verification email and caches the verification code.
        /// </summary>
        /// <param name="emailAddress">The email address to log in as.</param>
        /// <returns>An OkResult.</returns>
        [HttpPost]
        [Route("sendverificationcode")]
        public async Task<IActionResult> VerifyEmail([FromBody]string emailAddress)
        {
            var verificationCodeCacheKey = VerificationCodeCacheKey(emailAddress);

            string verificationCode = FursvpRandom.CopyableButHardToGuessCode();

            this.memoryCache.Set(verificationCodeCacheKey, verificationCode);

            var email = CreateVerificationEmail(emailAddress, verificationCode);

            await this.emailer.SendAsync(email);

            return this.Ok();
        }

        private static string VerificationCodeCacheKey(string emailAddress) => $"VerificationCodeFor:{emailAddress}";

        private static Email CreateVerificationEmail(string emailAddress, string verificationCode)
        {
            return new Email
            {
                From = new EmailAddress { Address = "noreply@fursvp.com", Name = "Fursvp" }, // TODO - make this a config variable
                To = new EmailAddress { Address = emailAddress },
                Subject = $"Your Code - {verificationCode}",
                PlainTextContent = @$"Hi!

Your Fursvp verification code is: {verificationCode}. Use it to verify your email in Fursvp.

If you didn't request this, simply ignore this message.",
                HtmlContent = @$"<p>Hi!</p>
<p>Your Fursvp verification code is <strong>{verificationCode}</strong>. Use it to verify your email in Fursvp.</p>
<p>If you didn't request this, simply ignore this message.</p>",
            };
        }

        private string CreateVerificationToken(string emailAddress)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.configuration["AuthorizationIssuerSigningKey"]);
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
            return writtenToken;
        }
    }
}
