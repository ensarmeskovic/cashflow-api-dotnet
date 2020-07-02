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
    public class ExpenseController : BaseController
    {
        public ExpenseController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region Add

        [HttpPost("add"), TokenValidation]
        public async Task<ActionResult> Add([FromBody] ExpenseAddRequest request)
        {
            LoggedUser loggedUser = LoggedUser;

            try
            {
                Expense expense = new Expense
                {
                    InboxId = request.InboxId,
                    UserId = loggedUser.UserId,
                    Name = request.Name,
                    TotalAmount = request.Amount,
                    Note = request.Note
                };

                IEnumerable<Payment> availableResources = await UnitOfWork.PaymentRepository.GetAvailableResourcesByInboxIdAsync(request.InboxId);

                decimal totalAmount = request.Amount;
                foreach (Payment payment in availableResources)
                {
                    if(totalAmount <= 0)
                        break;

                    decimal availablePayment = payment.TotalAmount - payment.CurrentAmount;
                    decimal paidAmount = availablePayment >= totalAmount ? totalAmount : availablePayment;

                    totalAmount -= paidAmount;
                    payment.CurrentAmount += paidAmount;

                    ExpensePayment expensePayment = new ExpensePayment
                    {
                        PaymentId = payment.Id,
                        ExpenseId = expense.Id,
                        Amount = paidAmount
                    };

                    UnitOfWork.PaymentRepository.Update(payment);
                    UnitOfWork.ExpensePaymentRepository.Add(expensePayment);
                }

                UnitOfWork.ExpenseRepository.Add(expense);
                await UnitOfWork.SaveChangesAsync();

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
        //public async Task<ActionResult> Edit([FromBody] ExpenseEditRequest request)
        //{
        //    LoggedUser loggedUser = LoggedUser;

        //    try
        //    {
        //        Expense expense = await UnitOfWork.ExpenseRepository.GetByIdAsync(request.ExpenseId);

        //        expense.Name = request.Name;
        //        expense.TotalAmount = request.TotalAmount;
        //        expense.Note = request.Note;
        //        expense.ModifiedUserId = loggedUser.UserId;

        //        UnitOfWork.ExpenseRepository.Update(expense);
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
                IEnumerable<Payment> payments = await UnitOfWork.PaymentRepository.GetByIdsAsync(expensePayments.Select(x => x.PaymentId).ToArray());

                foreach (Payment payment in payments)
                {
                    payment.CurrentAmount -= expensePayments.Where(x => x.ExpenseId == id && x.PaymentId == payment.Id).Sum(x => x.Amount);
                }

                UnitOfWork.PaymentRepository.UpdateRange(payments.AsQueryable());

                Expense expense = UnitOfWork.ExpenseRepository.RemoveById(id);
                if (expense is null)
                    return BadRequest("Expense does not exist!");

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