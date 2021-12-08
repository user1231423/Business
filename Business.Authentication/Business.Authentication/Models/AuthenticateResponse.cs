namespace Business.Authentication.Models
{
    using Data.Authentication.Models;
    using System;

    public class AuthenticateResponse
    {
        /// <summary>
        /// Valid from
        /// </summary>
        public DateTime ValidFrom { get; set; }

        /// <summary>
        /// Valid to
        /// </summary>
        public DateTime ValidTo { get; set; }

        /// <summary>
        /// Expires in
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Token type
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
    }
}
