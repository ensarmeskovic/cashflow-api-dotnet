using System;
using System.Threading.Tasks;
using Cashflow.Api.Filters;
using Cashflow.Domain.DomainObjects;
using Cashflow.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace Cashflow.Api.Controllers.Inboxes
{
    [ApiController, Route("[controller]")]
    public class InboxUserController : BaseController
    {
        public InboxUserController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet("seen"), TokenValidation]
        public async Task<ActionResult> Seen(int inboxId)
        {
            LoggedUser loggedUser = LoggedUser;

            try
            {
                InboxUser inboxUser = await UnitOfWork.InboxUserRepository.GetByUserIdAndInboxIdAsync(loggedUser.UserId, inboxId);

                inboxUser.SeenDateTime = DateTime.Now;

                UnitOfWork.InboxUserRepository.Update(inboxUser);
                await UnitOfWork.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex.InnerExceptionMessage());
            }
        }
    }
}