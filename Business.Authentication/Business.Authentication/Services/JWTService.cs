namespace Business.Authentication.Services
{
    using Business.Authentication.Interfaces;
    using Business.Authentication.Models;
    using Common.Encoding.Hash;
    using Common.Extensions;
    using Data.Authentication.Models;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Configuration;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class JwtService : IJwtService
    {
        /// <summary>
        /// Jwt settings
        /// </summary>
        private readonly IJwtSettings jwtSettings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jwtSettings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public JwtService(IJwtSettings jwtSettings)
        {
            this.jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        /// <summary>
        /// Generate JWT Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public AuthenticateResponse GenerateJwtToken(User user, int validTime)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            byte[] key = Convert.FromBase64String(jwtSettings.Secret);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Email", user.Email),
                }),
                TokenType = "Bearer",
                Expires = validTime == 0 ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddSeconds(validTime), //If valid time is 0 token is valid for 7 days by default
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            AuthenticateResponse authRepsonse = new()
            {
                Token = tokenHandler.WriteToken(token),
                ExpiresIn = token.ValidFrom.GetLifetimeInSeconds(token.ValidTo),
                TokenType = "Bearer",
                ValidFrom = token.ValidFrom,
                ValidTo = token.ValidTo
            };

            return authRepsonse;
        }
    }
}
