using Business.Authentication.Services;
using Common.Encoding.Hash;
using Data.Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Authentication.Middleware
{
    public class Authorize : Attribute, IAuthorizationFilter
    {
        private readonly UserService _userService;

        public Authorize(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// On authorization
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (token == null)
                    // Token missing from headers
                    context.Result = new UnauthorizedObjectResult(new { message = "Missing Bearer Token" });

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["Secret"].ToMD5());
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                if (jwtToken == null)
                    context.Result = new UnauthorizedObjectResult(new { message = "Invalid token" });

                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value);

                // Attach user to context on successful jwt validation
                context.HttpContext.Items["User"] = _userService.Load(userId);
            }catch(Exception e)
            {
                context.Result = new BadRequestObjectResult(new { message = "Problem deserializing token" });
            }
        }
    }
}
