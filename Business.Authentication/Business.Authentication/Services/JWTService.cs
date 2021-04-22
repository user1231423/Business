namespace Business.Authentication.Services
{
    using Common.Encoding.Hash;
    using Data.Authentication.Models;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Configuration;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public static class JWTService
    {
        /// <summary>
        /// Generate JWT Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GenerateJwtToken(User user, int validTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Convert.FromBase64String(ConfigurationManager.AppSettings["Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("Id", user.Id.ToString()) }),
                Expires = validTime == 0 ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddSeconds(validTime), //If valid time is 0 token is valid for 7 days by default
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
