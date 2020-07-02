using System.ComponentModel.DataAnnotations;

namespace Cashflow.Api.Controllers.Entries.Requests
{
    public class PaymentAddRequest
    {
        [Required]
        public int InboxId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Note { get; set; }
    }
}
