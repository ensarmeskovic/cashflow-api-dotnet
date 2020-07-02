using System.Threading.Tasks;
using Cashflow.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;

namespace Cashflow.Infrastructure.Repositories.Inboxes
{
    public class InboxUserRepository : Repository<InboxUser, int>, IInboxUserRepository
    {
        public InboxUserRepository(CashflowContext context) : base(context)
        {
        }

        public InboxUser GetByUserIdAndInboxId(int userId, int inboxId)
        {
            return GetByUserIdAndInboxIdAsync(userId, inboxId).Result;
        }
        public async Task<InboxUser> GetByUserIdAndInboxIdAsync(int userId, int inboxId)
        {
            return await Context.InboxUsers.FirstOrDefaultAsync(x => x.UserId == userId && x.InboxId == inboxId);
        }
    }
}
