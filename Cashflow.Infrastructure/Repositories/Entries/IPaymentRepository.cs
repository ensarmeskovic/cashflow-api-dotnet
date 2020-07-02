using System.Collections.Generic;
using System.Threading.Tasks;
using Cashflow.Domain.DataTransferObjects;
using Cashflow.Domain.DomainObjects;

namespace Cashflow.Infrastructure.Repositories.Entries
{
    public interface IPaymentRepository : IRepository<Payment, int>
    {
        IEnumerable<EntryDto> GetByInboxId(int inboxId);
        Task<IEnumerable<EntryDto>> GetByInboxIdAsync(int inboxId);

        IEnumerable<Payment> GetByIds(int[] ids);
        Task<IEnumerable<Payment>> GetByIdsAsync(int[] ids);


        IEnumerable<Payment> GetAvailableResourcesByInboxId(int inboxId);
        Task<IEnumerable<Payment>> GetAvailableResourcesByInboxIdAsync(int inboxId);
    }
}
