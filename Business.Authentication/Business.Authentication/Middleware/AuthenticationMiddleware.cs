namespace Business.Authentication.Middleware
{
    using Business.Authentication.Interfaces;
    using Business.Authentication.Services;
    using Common.Encoding.Hash;
    using Common.Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Configuration;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Authentication middleware
    /// </summary>
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        // Dependency Injection
        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Executed on invocation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userService"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IUserService userService, IJwtSettings jwtSettings)
        {
            //Reading the AuthHeader which is signed with JWT
            string token = context.GetAuthorizationHeader();

            if (token != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Convert.FromBase64String(jwtSettings.Secret);
                var jwtToken = new JwtSecurityToken();

                //If validate token fails then jwt Token is inválid
                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        // Set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    jwtToken = (JwtSecurityToken)validatedToken;
                }
                catch
                {
                    jwtToken = null;
                }

                if (jwtToken != null)
                {

                    var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value);

                    var user = await userService.LoadAsync(userId);
                    // Attach user to context on successful jwt validation
                    //context.Items["User"] = user;


                    // Identity Principal
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.AuthenticationMethod, "Jwt"),
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                        new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    var identity = new ClaimsIdentity(claims, "Basic");
                    context.User = new ClaimsPrincipal(identity);
                }
            }

            //Pass to the next middleware
            await _next(context);
        }
    }
}
