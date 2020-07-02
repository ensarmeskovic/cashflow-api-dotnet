using System.Collections.Generic;
using Cashflow.Domain.DomainObjects;

namespace Cashflow.Infrastructure.Repositories.Entries
{
    public interface IExpensePaymentRepository : IRepository<ExpensePayment, int>
    {
        IEnumerable<ExpensePayment> RemoveByPaymentId(int paymentId);
        IEnumerable<ExpensePayment> RemoveByExpenseId(int expenseId);
    }
}
