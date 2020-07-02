using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cashflow.Domain.DataTransferObjects;
using Cashflow.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;

namespace Cashflow.Infrastructure.Repositories.Entries
{
    public class ExpenseRepository : Repository<Expense, int>, IExpenseRepository
    {
        public ExpenseRepository(CashflowContext context) : base(context)
        {
        }

        public IEnumerable<EntryDto> GetByInboxId(int inboxId)
        {
            return GetByInboxIdAsync(inboxId).Result;
        }
        public async Task<IEnumerable<EntryDto>> GetByInboxIdAsync(int inboxId)
        {
            return await Context.Expenses.Include(x => x.User).Where(x => x.InboxId == inboxId && !x.DeletedDateTime.HasValue).Select(x =>
            new EntryDto
            {
                ExpenseId = x.Id,
                Username = x.User.DisplayName,
                Name = x.Name,
                CurrentAmount = x.CurrentAmount,
                TotalAmount = x.TotalAmount,
                Date = x.AddedDateTime.Date,
                Note = x.Note
            }).ToListAsync();
        }

        public IEnumerable<Expense> GetByIds(int[] ids)
        {
            return GetByIdsAsync(ids).Result;
        }
        public async Task<IEnumerable<Expense>> GetByIdsAsync(int[] ids)
        {
            return await Context.Expenses.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public IEnumerable<Expense> GetNotPaidExpensesByInboxId(int inboxId)
        {
            return GetNotPaidExpensesByInboxIdAsync(inboxId).Result;
        }
        public async Task<IEnumerable<Expense>> GetNotPaidExpensesByInboxIdAsync(int inboxId)
        {
            return await Context.Expenses.Where(x => x.InboxId == inboxId && !x.DeletedDateTime.HasValue && x.CurrentAmount < x.TotalAmount).OrderBy(x => x.AddedDateTime).ToListAsync();
        }
    }
}
