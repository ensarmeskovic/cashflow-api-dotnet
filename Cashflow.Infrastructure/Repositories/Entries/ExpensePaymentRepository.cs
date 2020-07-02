using System.Collections.Generic;
using System.Linq;
using Cashflow.Domain.DomainObjects;

namespace Cashflow.Infrastructure.Repositories.Entries
{
    public class ExpensePaymentRepository : Repository<ExpensePayment, int>, IExpensePaymentRepository
    {
        public ExpensePaymentRepository(CashflowContext context) : base(context)
        {
        }

        public IEnumerable<ExpensePayment> RemoveByPaymentId(int paymentId)
        {
            IQueryable<ExpensePayment> expensePayments = Context.ExpensePayments.Where(x => x.PaymentId == paymentId);
            Context.RemoveRange(expensePayments);

            return expensePayments;
        }

        public IEnumerable<ExpensePayment> RemoveByExpenseId(int expenseId)
        {
            IQueryable<ExpensePayment> expensePayments = Context.ExpensePayments.Where(x => x.ExpenseId == expenseId);
            Context.RemoveRange(expensePayments);

            return expensePayments;
        }
    }
}
