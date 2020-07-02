using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cashflow.Domain.DomainObjects
{
    public class ExpensePayment : IEntity
    {
        public int Id { get; set; }
        
        public int ExpenseId { get; set; }
        public int PaymentId { get; set; }

        public decimal Amount { get; set; }

        
        [ForeignKey(nameof(ExpenseId))]
        public Expense Expense { get; set; }
        [ForeignKey(nameof(PaymentId))]
        public Payment Payment { get; set; }


        public DateTime AddedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public DateTime? DeletedDateTime { get; set; }
    }
}
