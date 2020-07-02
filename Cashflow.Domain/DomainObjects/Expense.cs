using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cashflow.Domain.DomainObjects
{
    public class Expense : IEntity
    {
        public int Id { get; set; }

        public int InboxId { get; set; }
        public int UserId { get; set; }

        public string Name { get; set; }
        public string Note { get; set; }

        public decimal CurrentAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime AddedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public DateTime? DeletedDateTime { get; set; }


        [ForeignKey(nameof(InboxId))]
        public Inbox Inbox { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
