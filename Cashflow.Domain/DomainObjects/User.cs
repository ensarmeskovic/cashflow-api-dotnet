using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Cashflow.Domain.DomainObjects
{
    public class User : IEntity
    {
        [Key]
        public int Id { get; set; }

        public string DisplayName { get; set; }
        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public string RegistrationCode { get; set; }
        public bool RegistrationConfirmed { get; set; }

        public string Token { get; set; }
        public DateTime? TokenExpirationDateTime { get; set; }
        public string DeviceToken { get; set; }

        public DateTime AddedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public DateTime? DeletedDateTime { get; set; }



        public Claim[] GetUserClaims()
        {
            return new[]
            {
                new Claim($"{nameof(User)}{nameof(Id)}", Id.ToString()),
                new Claim(nameof(Email), Email)
            };
        }
    }
}
