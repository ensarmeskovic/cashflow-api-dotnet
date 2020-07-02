using System.ComponentModel.DataAnnotations;

namespace Cashflow.Api.Controllers.Users.Requests
{
    public class RegistrationRequest
    {
        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string PasswordConfirmation { get; set; }
    }
}
