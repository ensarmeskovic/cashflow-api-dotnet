using System.ComponentModel.DataAnnotations;

namespace Cashflow.Api.Controllers.Users.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordConfirmation { get; set; }
    }
}
