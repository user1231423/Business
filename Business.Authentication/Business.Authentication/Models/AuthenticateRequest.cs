namespace Business.Authentication.Models
{
    using Business.Authentication.Enums;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class AuthenticateRequest
    {
        /// <summary>
        /// Grant type enum
        /// </summary>
        [JsonIgnore]
        public GrantType GrantTypeEnum { get; protected set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Grant type
        /// </summary>
        public string GrantType
        {
            set
            {
                GrantTypeEnum = (GrantType)Enum.Parse(typeof(GrantType), value, true);
            }
        }
    }
}
