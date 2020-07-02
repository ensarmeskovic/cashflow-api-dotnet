using System;
using System.Threading.Tasks;
using Cashflow.Infrastructure.Repositories.Entries;
using Cashflow.Infrastructure.Repositories.Inboxes;
using Cashflow.Infrastructure.Repositories.Users;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cashflow.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        #region Save changes
        int SaveChanges();
        Task<int> SaveChangesAsync();
        #endregion

        #region Transactions
        void ExecuteTransaction(Action<IDbContextTransaction, int> prepere, Action commit = null, Action rollback = null);

        Task ExecuteTransactionAsync(Func<IDbContextTransaction, int, Task> prepare, Func<Task> commit = null, Func<Task> rollback = null);
        Task ExecuteTransactionAsync(Func<IDbContextTransaction, int, Task> prepare, Func<Task> commit = null, Action rollback = null);
        Task ExecuteTransactionAsync(Func<IDbContextTransaction, int, Task> prepare, Action commit = null, Func<Task> rollback = null);
        Task ExecuteTransactionAsync(Func<IDbContextTransaction, int, Task> prepare, Action commit = null, Action rollback = null);
        #endregion

        #region Repositories
        IUserRepository UserRepository { get; }
        IInboxRepository InboxRepository { get; }
        IInboxUserRepository InboxUserRepository { get; }
        IExpenseRepository ExpenseRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IExpensePaymentRepository ExpensePaymentRepository { get; }
        #endregion
    }
}
