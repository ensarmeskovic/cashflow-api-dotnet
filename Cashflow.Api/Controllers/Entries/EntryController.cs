using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cashflow.Api.Controllers.Entries.Responses;
using Cashflow.Api.Filters;
using Cashflow.Domain.DataTransferObjects;
using Cashflow.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace Cashflow.Api.Controllers.Entries
{
    [ApiController, Route("[controller]")]
    public class EntryController : BaseController
    {
        public EntryController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region Get inbox details

        [HttpGet, TokenValidation]
        public async Task<ActionResult<EntryResponse>> Get([FromQuery] int id)
        {
            try
            {
                IEnumerable<EntryDto> expenses = await UnitOfWork.ExpenseRepository.GetByInboxIdAsync(id);
                IEnumerable<EntryDto> payments = await UnitOfWork.PaymentRepository.GetByInboxIdAsync(id);

                IEnumerable<EntryDto> entries = expenses.Union(payments);

                EntryResponse entryResponse = new EntryResponse
                {
                    Entries = entries,
                    Expenses = entries.Where(x => x.ExpenseId.HasValue && !x.PaymentId.HasValue)
                        .Sum(x => x.TotalAmount),
                    Payments = entries.Where(x => x.PaymentId.HasValue && !x.ExpenseId.HasValue)
                        .Sum(x => x.TotalAmount)
                };

                return Ok(entryResponse);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex.InnerExceptionMessage());
            }
        }

        #endregion
    }
}