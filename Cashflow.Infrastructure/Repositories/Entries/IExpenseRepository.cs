using System.Collections.Generic;
using System.Threading.Tasks;
using Cashflow.Domain.DataTransferObjects;
using Cashflow.Domain.DomainObjects;

namespace Cashflow.Infrastructure.Repositories.Entries
{
    public interface IExpenseRepository : IRepository<Expense, int>
    {
        IEnumerable<EntryDto> GetByInboxId(int inboxId);
        Task<IEnumerable<EntryDto>> GetByInboxIdAsync(int inboxId);

        IEnumerable<Expense> GetByIds(int[] ids);
        Task<IEnumerable<Expense>> GetByIdsAsync(int[] ids);

        IEnumerable<Expense> GetNotPaidExpensesByInboxId(int inboxId);
        Task<IEnumerable<Expense>> GetNotPaidExpensesByInboxIdAsync(int inboxId);
    }
}
