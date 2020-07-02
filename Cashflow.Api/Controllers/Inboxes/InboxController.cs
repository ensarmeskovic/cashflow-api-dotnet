using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cashflow.Api.Controllers.Inboxes.Requests;
using Cashflow.Api.Filters;
using Cashflow.Domain.DataTransferObjects;
using Cashflow.Domain.DomainObjects;
using Cashflow.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace Cashflow.Api.Controllers.Inboxes
{
    [ApiController, Route("[controller]")]
    public class InboxController : BaseController
    {
        public InboxController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region Get 

        [HttpGet, TokenValidation]
        public async Task<ActionResult<IEnumerable<InboxDto>>> Get()
        {
            LoggedUser loggedUser = LoggedUser;

            try
            {
                IEnumerable<InboxDto> inboxes = await UnitOfWork.InboxRepository.GetByUserIdAsync(loggedUser.UserId);

                return Ok(inboxes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex.InnerExceptionMessage());
            }
        }

        #endregion

        #region Add 
        [HttpPost("add"), TokenValidation]
        public async Task<ActionResult> Add([FromBody] InboxAddRequest request)
        {
            LoggedUser loggedUser = LoggedUser;

            try
            {
                await UnitOfWork.ExecuteTransactionAsync(async (transaction, timeout) =>
                {
                    Inbox inbox = new Inbox
                    {
                        Name = request.Name,
                        AdminId = loggedUser.UserId,
                        Active = true
                    };

                    UnitOfWork.InboxRepository.Add(inbox);
                    await UnitOfWork.SaveChangesAsync();

                    InboxUser inboxUser = new InboxUser
                    {
                        UserId = loggedUser.UserId,
                        InboxId = inbox.Id
                    };

                    UnitOfWork.InboxUserRepository.Add(inboxUser);
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
        [HttpPost("edit"), TokenValidation]
        public async Task<ActionResult> Edit([FromBody] InboxEditRequest request)
        {
            try
            {
                Inbox inbox = await UnitOfWork.InboxRepository.GetByIdAsync(request.InboxId);
                if (inbox is null)
                    return BadRequest("Inbox does not exist!");

                await UnitOfWork.ExecuteTransactionAsync(async (transaction, timeout) =>
                {
                    inbox.Name = request.Name;

                    UnitOfWork.InboxRepository.Update(inbox);
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

        #region Deactivate or activate
        [HttpPut("de-activate"), TokenValidation]
        public async Task<ActionResult> DeActivate(int id)
        {
            try
            {
                Inbox inbox = await UnitOfWork.InboxRepository.GetByIdAsync(id);
                if (inbox is null)
                    return BadRequest("Inbox does not exist!");

                await UnitOfWork.ExecuteTransactionAsync(async (transaction, timeout) =>
                {
                    inbox.Active = !inbox.Active;

                    UnitOfWork.InboxRepository.Update(inbox);
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

        #region Delete
        [HttpDelete("delete"), TokenValidation]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Inbox inbox = UnitOfWork.InboxRepository.RemoveById(id);
                if (inbox is null)
                    return BadRequest("Inbox does not exist!");

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