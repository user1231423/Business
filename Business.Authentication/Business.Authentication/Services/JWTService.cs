namespace Business.Authentication.Services
{
    using Business.Authentication.Interfaces;
    using Business.Authentication.Models;
    using Common.Cache.Interfaces;
    using Common.Encoding.Hash;
    using Common.Extensions;
    using Data.Authentication.Models;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Configuration;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public class JwtService : IJwtService
    {
        /// <summary>
        /// Jwt settings
        /// </summary>
        private readonly IJwtSettings _jwtSettings;

        /// <summary>
        /// Cache service
        /// </summary>
        private readonly IRedisCacheService _cacheService;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jwtSettings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public JwtService(IJwtSettings jwtSettings, IRedisCacheService cacheService)
        {
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        /// <summary>
        /// Generate JWT Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<AuthenticateResponse> GenerateJwtToken(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            byte[] key = Convert.FromBase64String(_jwtSettings.Secret);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Email", user.Email),
                }),
                TokenType = "Bearer",
                Expires = _jwtSettings.Seconds == 0 ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddSeconds(_jwtSettings.Seconds), //If valid time is 0 token is valid for 7 days by default
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            AuthenticateResponse authRepsonse = new AuthenticateResponse()
            {
                AccessToken = tokenHandler.WriteToken(token),
                ExpiresIn = token.ValidFrom.GetLifetimeInSeconds(token.ValidTo),
                Created = token.ValidFrom,
                Expiration = token.ValidTo,
                RefreshToken = await _cacheService.Set(new
                {
                    user.Id,
                    user.Email
                }, new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = token.ValidTo
                })
            };

            return authRepsonse;
        }
    }
}
