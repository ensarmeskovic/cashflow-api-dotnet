using System.ComponentModel.DataAnnotations;

namespace Cashflow.Api.Controllers.Entries.Requests
{
    public class PaymentEditRequest
    {
        [Required]
        public int ExpenseId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Note { get; set; }
    }
}
