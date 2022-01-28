namespace Business.Authentication.Models
{
    using Data.Authentication.Models;
    using System;

    public class AuthenticateResponse
    {
        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Expiration date
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Expires in
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Token type
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
