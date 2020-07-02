using System;
using System.Collections.Generic;
using System.Text;

namespace Cashflow.Domain
{
    public interface IEntity
    {
        int Id { get; set; }

        DateTime AddedDateTime { get; set; }
        DateTime? ModifiedDateTime { get; set; }
        DateTime? DeletedDateTime { get; set; }
    }
}
