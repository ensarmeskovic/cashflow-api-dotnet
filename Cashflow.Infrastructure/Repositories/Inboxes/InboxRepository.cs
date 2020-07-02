using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashflow.Domain.DataTransferObjects;
using Cashflow.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;

namespace Cashflow.Infrastructure.Repositories.Inboxes
{
    public class InboxRepository : Repository<Inbox, int>, IInboxRepository
    {
        public InboxRepository(CashflowContext context) : base(context)
        {
        }

        public IEnumerable<InboxDto> GetByUserId(int userId)
        {
            return GetByUserIdAsync(userId).Result;
        }
        public async Task<IEnumerable<InboxDto>> GetByUserIdAsync(int userId)
        {
            return Context.InboxUsers.Include(x => x.Inbox).AsNoTracking()
                .Where(x => x.UserId == userId && !x.Inbox.DeletedDateTime.HasValue).Select(x => new InboxDto
                {
                    InboxId = x.InboxId,
                    Name = x.Inbox.Name,
                    Active = x.Inbox.Active,
                    SeenAll = false //ToDo: ...
                }).OrderByDescending(x => x.SeenAll);
        }
    }
}
