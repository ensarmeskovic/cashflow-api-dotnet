using System.Collections.Generic;
using Cashflow.Domain.DataTransferObjects;

namespace Cashflow.Api.Controllers.Entries.Responses
{
    public class EntryResponse
    {
        public decimal Expenses { get; set; }
        public  decimal Payments { get; set; }

        public decimal Balance => Payments - Expenses;
        
        public IEnumerable<EntryDto> Entries { get; set; }
    }
}
