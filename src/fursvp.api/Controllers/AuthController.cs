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
    using Fursvp.Api.Requests;
    using Fursvp.Communication;
    using Fursvp.Domain.Authorization;
    using Fursvp.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Authentication controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="memoryCache">The application memory cache.</param>
        /// <param name="emailer">The instance of <see cref="IEmailer"/> used to suppress or send emails.</param>
        public AuthController(IConfiguration configuration, IMemoryCache memoryCache, IEmailer emailer, IUserAccessor userAccessor)
        {
            Configuration = configuration;
            MemoryCache = memoryCache;
            Emailer = emailer;
            UserAccessor = userAccessor;
        }

        private IConfiguration Configuration { get; }

        private IMemoryCache MemoryCache { get; }

        private IEmailer Emailer { get; }
        
        private IUserAccessor UserAccessor { get; }

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
            var token = CreateVerificationToken(emailAddress, out var sessionId);
            MemoryCache.Set("SessionId:" + sessionId, true);
            return Ok(token);
        }

        /// <summary>
        /// Throws an exception.
        /// </summary>
        /// <returns>Always throws an exception.</returns>
        [ServiceFilter(typeof(DebugModeOnlyFilter))]
        [HttpPost]
        [Route("debugerror")]
        public IActionResult DebugError()
        {
            throw new Exception();
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
            if (verifyEmailRequest == null)
            {
                return BadRequest();
            }

            var verificationCodeCacheKey = VerificationCodeCacheKey(verifyEmailRequest.EmailAddress);

            if (MemoryCache.TryGetValue(verificationCodeCacheKey, out string verificationCode))
            {
                if (verificationCode == verifyEmailRequest.VerificationCode)
                {
                    // authentication successful.
                    ExpireVerificationCode(verifyEmailRequest.EmailAddress);
                    var token = CreateVerificationToken(verifyEmailRequest.EmailAddress, out string sessionId);
                    MemoryCache.Set("SessionId:" + sessionId, true);
                    return Ok(token);
                }

                IncrementFailedVerificationAttemptsOrExpire(verifyEmailRequest.EmailAddress);
            }

            return Unauthorized();
        }

        [HttpDelete]
        [Route("logout")]
        public IActionResult LogOut()
        {
            var user = UserAccessor.User;

            if (user != null && user.SessionId != null)
            {
                MemoryCache.Remove("SessionId:" + user.SessionId);
            }

            return Ok();
        }

        /// <summary>
        /// Sends a verification email and caches the verification code.
        /// </summary>
        /// <param name="sendVerificationCodeRequest">The email address to log in as.</param>
        /// <returns>An OkResult on success or BadRequestResult on failure.</returns>
        [HttpPost]
        [Route("sendverificationcode")]
        public async Task<IActionResult> SendVerificationCode([FromBody]SendVerificationCodeRequest sendVerificationCodeRequest)
        {
            if (sendVerificationCodeRequest == null)
            {
                return BadRequest();
            }

            var verificationCodeCacheKey = VerificationCodeCacheKey(sendVerificationCodeRequest.EmailAddress);

            string verificationCode = FursvpRandom.CopyableButHardToGuessCode();

            MemoryCache.Set(verificationCodeCacheKey, verificationCode, TimeSpan.FromMinutes(60)); // TODO - make this a config variable

            var email = CreateVerificationEmail(sendVerificationCodeRequest.EmailAddress, verificationCode);

            await Emailer.SendAsync(email).ConfigureAwait(false);

            return Ok();
        }

        private static string VerificationCodeCacheKey(string emailAddress) => $"VerificationCodeFor:{emailAddress}";

        private static string VerificationAttemptsCacheKey(string emailAddress) => $"VerificationAttemptsFor:{emailAddress}";

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

        private void ExpireVerificationCode(string emailAddress)
        {
            MemoryCache.Remove(VerificationCodeCacheKey(emailAddress));
            MemoryCache.Remove(VerificationAttemptsCacheKey(emailAddress));
        }

        private string CreateVerificationToken(string emailAddress, out string sessionId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["AuthorizationIssuerSigningKey"] ?? "00000000-0000-0000-0000-000000000000");
            sessionId = Guid.NewGuid().ToString();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, emailAddress),
                    new Claim("sessionid", sessionId),
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);
            var writtenToken = tokenHandler.WriteToken(createdToken);
            return writtenToken;
        }

        private void IncrementFailedVerificationAttemptsOrExpire(string emailAddress)
        {
            var verificationAttemptsCacheKey = VerificationAttemptsCacheKey(emailAddress);
            MemoryCache.TryGetValue(verificationAttemptsCacheKey, out int failedAttempts);
            failedAttempts++;

            // TODO - make this a config variable
            if (failedAttempts >= 5)
            {
                // We've hit the max allowed verification code attempts.
                ExpireVerificationCode(emailAddress);
            }
            else
            {
                MemoryCache.Set(verificationAttemptsCacheKey, failedAttempts, TimeSpan.FromMinutes(15)); // TODO - make this a config variable.
            }
        }
    }
}
