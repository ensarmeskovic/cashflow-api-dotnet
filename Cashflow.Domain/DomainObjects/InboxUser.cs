using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cashflow.Domain.DomainObjects
{
    public class InboxUser : IEntity
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int InboxId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? SeenDateTime { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        [ForeignKey(nameof(InboxId))]
        public Inbox Inbox { get; set; }



        public DateTime AddedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public DateTime? DeletedDateTime { get; set; }
    }
}
