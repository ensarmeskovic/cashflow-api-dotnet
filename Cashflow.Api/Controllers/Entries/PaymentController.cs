using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cashflow.Api.Controllers.Entries.Requests;
using Cashflow.Api.Filters;
using Cashflow.Domain.DomainObjects;
using Cashflow.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace Cashflow.Api.Controllers.Entries
{
    [ApiController, Route("[controller]")]
    public class PaymentController : BaseController
    {
        public PaymentController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region Add

        [HttpPost("add"), TokenValidation]
        public async Task<ActionResult> Add([FromBody] PaymentAddRequest request)
        {
            LoggedUser loggedUser = LoggedUser;

            try
            {
                await UnitOfWork.ExecuteTransactionAsync(async (transaction, timeout) =>
                {
                    Payment payment = new Payment
                    {
                        InboxId = request.InboxId,
                        UserId = loggedUser.UserId,
                        TotalAmount = request.Amount,
                        Note = request.Note
                    };

                    UnitOfWork.PaymentRepository.Add(payment);
                    await UnitOfWork.SaveChangesAsync();

                    IEnumerable<Expense> expensesNotPaid = await UnitOfWork.ExpenseRepository.GetNotPaidExpensesByInboxIdAsync(request.InboxId);

                    decimal totalPayment = payment.TotalAmount;
                    foreach (Expense expense in expensesNotPaid)
                    {
                        if (totalPayment <= 0)
                            break;

                        decimal amountToPay = expense.TotalAmount - expense.CurrentAmount;
                        decimal paidAmount = totalPayment >= amountToPay ? amountToPay : totalPayment;

                        totalPayment -= paidAmount;
                        expense.CurrentAmount += paidAmount;

                        ExpensePayment expensePayment = new ExpensePayment
                        {
                            PaymentId = payment.Id,
                            ExpenseId = expense.Id,
                            Amount = paidAmount
                        };

                        UnitOfWork.ExpenseRepository.Update(expense);
                        UnitOfWork.ExpensePaymentRepository.Add(expensePayment);
                    }

                    payment.CurrentAmount = payment.TotalAmount - totalPayment;
                    UnitOfWork.PaymentRepository.Update(payment);

                    await UnitOfWork.SaveChangesAsync();
                }, null, null);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex.InnerExceptionMessage());
            }
        }

        #endregion

        #region Edit

        //[HttpPost("edit"), TokenValidation]
        //public async Task<ActionResult> Edit([FromBody] PaymentEditRequest request)
        //{
        //    LoggedUser loggedUser = LoggedUser;

        //    try
        //    {
        //        Payment payment = await UnitOfWork.PaymentRepository.GetByIdAsync(request.ExpenseId);

        //        payment.Note = request.Note;
        //        payment.TotalAmount = request.TotalAmount;
        //        payment.ModifiedUserId = loggedUser.UserId;

        //        UnitOfWork.PaymentRepository.Update(payment);
        //        await UnitOfWork.SaveChangesAsync();

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log(LogLevel.Error, ex.InnerExceptionMessage());

        //        return InternalServerError(ex.InnerExceptionMessage());
        //    }
        //}

        #endregion

        #region Delete

        [HttpDelete("delete"), TokenValidation]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                IEnumerable<ExpensePayment> expensePayments = UnitOfWork.ExpensePaymentRepository.RemoveByPaymentId(id);
                IEnumerable<Expense> expenses = await UnitOfWork.ExpenseRepository.GetByIdsAsync(expensePayments.Select(x => x.ExpenseId).ToArray());

                foreach (Expense expense in expenses)
                {
                    expense.CurrentAmount -= expensePayments.Where(x => x.PaymentId == id && x.ExpenseId == expense.Id).Sum(x => x.Amount);
                }

                UnitOfWork.ExpenseRepository.UpdateRange(expenses.AsQueryable());
                Payment payment = UnitOfWork.PaymentRepository.RemoveById(id);

                await UnitOfWork.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex.InnerExceptionMessage());
            }
        }

        #endregion
    }
}