using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Cashflow.Infrastructure.Repositories.Entries;
using Cashflow.Infrastructure.Repositories.Inboxes;
using Cashflow.Infrastructure.Repositories.Users;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cashflow.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly int _connectionTimeout;
        private readonly CashflowContext _cashflowContext;
        public IDbContextTransaction Transaction => _cashflowContext.Database.CurrentTransaction ?? _cashflowContext.Database.BeginTransaction();
        public UnitOfWork(CashflowContext cashflowContext, int connectionTimeout = 5)
        {
            _cashflowContext = cashflowContext;
            _connectionTimeout = connectionTimeout;
        }

        #region Save changes
        public int SaveChanges() => _cashflowContext.SaveChanges();

        public async Task<int> SaveChangesAsync() => await _cashflowContext.SaveChangesAsync();
        #endregion

        #region Transaction
        /// <summary>
        /// DO NOT USE THIS METHOD WITH ASYNC AND AWAIT :: GOOGLE - ACTION DELEGATE TO LEARN MORE!
        /// </summary>
        public void ExecuteTransaction(Action<IDbContextTransaction, int> prepare, Action commit = null, Action rollback = null)
        {
            try
            {
                prepare.Invoke(Transaction, _connectionTimeout);

                Transaction.Commit();

                commit?.Invoke();
            }
            catch (Exception ex)
            {
                Transaction.Rollback();
                Debug.WriteLine(ex.ToString());

                rollback?.Invoke();

                throw;
            }
        }
        public async Task ExecuteTransactionAsync(Func<IDbContextTransaction, int, Task> prepare, Func<Task> commit = null, Func<Task> rollback = null)
        {
            try
            {
                await prepare(Transaction, _connectionTimeout);

                Transaction.Commit();

                if (commit != null)
                    await commit();
            }
            catch (Exception ex)
            {
                Transaction.Rollback();
                Debug.WriteLine(ex.ToString());

                if (rollback != null)
                    await rollback();

                throw;
            }
        }
        public async Task ExecuteTransactionAsync(Func<IDbContextTransaction, int, Task> prepare, Func<Task> commit = null, Action rollback = null)
        {
            try
            {
                await prepare(Transaction, _connectionTimeout);

                Transaction.Commit();

                if (commit != null)
                    await commit();
            }
            catch (Exception ex)
            {
                Transaction.Rollback();
                Debug.WriteLine(ex.ToString());

                rollback?.Invoke();

                throw;
            }
        }
        public async Task ExecuteTransactionAsync(Func<IDbContextTransaction, int, Task> prepare, Action commit = null, Func<Task> rollback = null)
        {
            try
            {
                await prepare(Transaction, _connectionTimeout);

                Transaction.Commit();

                commit?.Invoke();
            }
            catch (Exception ex)
            {
                Transaction.Rollback();
                Debug.WriteLine(ex.ToString());

                if (rollback != null)
                    await rollback();

                throw;
            }
        }
        public async Task ExecuteTransactionAsync(Func<IDbContextTransaction, int, Task> prepare, Action commit = null, Action rollback = null)
        {
            try
            {
                await prepare(Transaction, _connectionTimeout);

                Transaction.Commit();

                commit?.Invoke();
            }
            catch (Exception ex)
            {
                Transaction.Rollback();
                Debug.WriteLine(ex.ToString());

                rollback?.Invoke();

                throw;
            }
        }
        #endregion

        #region Repoositories
        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_cashflowContext);

        private IInboxRepository _inboxRepository;
        public IInboxRepository InboxRepository => _inboxRepository ??= new InboxRepository(_cashflowContext);

        private IInboxUserRepository _inboxUserRepository;
        public IInboxUserRepository InboxUserRepository => _inboxUserRepository ??= new InboxUserRepository(_cashflowContext);

        private IExpenseRepository _expenseRepository;
        public IExpenseRepository ExpenseRepository => _expenseRepository ??= new ExpenseRepository(_cashflowContext);

        private IPaymentRepository _paymentRepository;
        public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_cashflowContext);

        private IExpensePaymentRepository _expensePaymentRepository;
        public IExpensePaymentRepository ExpensePaymentRepository => _expensePaymentRepository ??= new ExpensePaymentRepository(_cashflowContext);
        #endregion
    }
}
