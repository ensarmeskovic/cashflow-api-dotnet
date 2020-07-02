using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cashflow.Domain.DomainObjects
{
    public class Inbox : IEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public bool Personal { get; set; }

        public int AdminId { get; set; }

        [ForeignKey(nameof(AdminId))]
        public User Admin { get; set; }

        public bool Active { get; set; }

        public DateTime AddedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public DateTime? DeletedDateTime { get; set; }
    }
}
