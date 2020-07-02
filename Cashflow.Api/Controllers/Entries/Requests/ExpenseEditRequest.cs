using System.ComponentModel.DataAnnotations;

namespace Cashflow.Api.Controllers.Entries.Requests
{
    public class ExpenseEditRequest
    {
        [Required]
        public int ExpenseId { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        public string Note { get; set; }
    }
}
