using System;
using System.ComponentModel.DataAnnotations;

namespace Cashflow.Domain.DataTransferObjects
{
    public class EntryDto
    {
        public int? ExpenseId { get; set; }
        public int? PaymentId { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public decimal CurrentAmount { get; set; }
        public decimal TotalAmount { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public string Note { get; set; }
    }
}
