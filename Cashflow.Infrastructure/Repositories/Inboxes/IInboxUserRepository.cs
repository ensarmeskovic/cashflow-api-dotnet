using System.Threading.Tasks;
using Cashflow.Domain.DomainObjects;

namespace Cashflow.Infrastructure.Repositories.Inboxes
{
    public interface IInboxUserRepository : IRepository<InboxUser, int>
    {
        InboxUser GetByUserIdAndInboxId(int userId, int inboxId);
        Task<InboxUser> GetByUserIdAndInboxIdAsync(int userId, int inboxId);
    }
}
