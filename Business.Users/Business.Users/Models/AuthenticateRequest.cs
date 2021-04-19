namespace Business.Users.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AuthenticateRequest
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Must be a valid email")]
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        /// <summary>
        /// Valid time
        /// </summary>
        [Required(ErrorMessage = "Valid time is required")]
        [Range(0, 2147483647, ErrorMessage = "Min value is 0 and Max value is 2147483647")]
        public int ValidTime { get; set; }
    }
}
