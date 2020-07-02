using System;

namespace Cashflow.Domain.DataTransferObjects
{
    public class UserTokenDto
    {
        public int UserId { get; set; }
        public DateTime? TokenExpirationDateTime { get; set; }
    }
}
