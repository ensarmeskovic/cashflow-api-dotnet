using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cashflow.Domain.DataTransferObjects;
using Cashflow.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;

namespace Cashflow.Infrastructure.Repositories.Entries
{
    public class PaymentRepository : Repository<Payment, int>, IPaymentRepository
    {
        public PaymentRepository(CashflowContext context) : base(context)
        {
        }

        public IEnumerable<EntryDto> GetByInboxId(int inboxId)
        {
            return GetByInboxIdAsync(inboxId).Result;
        }
        public async Task<IEnumerable<EntryDto>> GetByInboxIdAsync(int inboxId)
        {
            return await Context.Payments.Include(x => x.User).Where(x => x.InboxId == inboxId && !x.DeletedDateTime.HasValue).Select(x =>
            new EntryDto
            {
                PaymentId = x.Id,
                Username = x.User.DisplayName,
                CurrentAmount = x.CurrentAmount,
                TotalAmount = x.TotalAmount,
                Date = x.AddedDateTime.Date,
                Note = x.Note
            }).ToListAsync();
        }

        public IEnumerable<Payment> GetByIds(int[] ids)
        {
            return GetByIdsAsync(ids).Result;
        }
        public async Task<IEnumerable<Payment>> GetByIdsAsync(int[] ids)
        {
            return await Context.Payments.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public IEnumerable<Payment> GetAvailableResourcesByInboxId(int inboxId)
        {
            return GetAvailableResourcesByInboxIdAsync(inboxId).Result;
        }
        public async Task<IEnumerable<Payment>> GetAvailableResourcesByInboxIdAsync(int inboxId)
        {
            return await Context.Payments
                .Where(x => x.InboxId == inboxId && !x.DeletedDateTime.HasValue && x.CurrentAmount < x.TotalAmount)
                .OrderBy(x => x.AddedDateTime).ToListAsync();
        }
    }
}
