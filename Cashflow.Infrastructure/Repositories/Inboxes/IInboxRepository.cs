using System.Collections.Generic;
using System.Threading.Tasks;
using Cashflow.Domain.DataTransferObjects;
using Cashflow.Domain.DomainObjects;

namespace Cashflow.Infrastructure.Repositories.Inboxes
{
    public interface IInboxRepository : IRepository<Inbox, int>
    {
        IEnumerable<InboxDto> GetByUserId(int userId);
        Task<IEnumerable<InboxDto>> GetByUserIdAsync(int userId);
    }
}
