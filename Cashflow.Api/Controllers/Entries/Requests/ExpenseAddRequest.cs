using System.ComponentModel.DataAnnotations;

namespace Cashflow.Api.Controllers.Entries.Requests
{
    public class ExpenseAddRequest
    {
        [Required]
        public int InboxId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Amount { get; set; }
        
        public string Note { get; set; }
    }
}
