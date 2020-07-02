using System;
using System.Linq;
using System.Security.Claims;
using Cashflow.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cashflow.Api.Controllers
{
    #region Claim user
    public class LoggedUser
    {
        public LoggedUser(Claim[] claims)
        {
            UserId = int.Parse(claims.FirstOrDefault(x => x.Type.Equals(nameof(UserId)))?.Value ?? throw new InvalidOperationException());
            Email = claims.FirstOrDefault(x => x.Type.Equals(nameof(Email)))?.Value ?? throw new InvalidOperationException();
        }

        public int UserId { get; set; }
        public string Email { get; set; }
    }
    #endregion

    [ApiController]
    public class BaseController : ControllerBase
    {
        protected LoggedUser LoggedUser => HttpContext.User.Claims.Any() ? new LoggedUser(HttpContext.User.Claims.ToArray()) : null;

        protected readonly IUnitOfWork UnitOfWork;

        public BaseController(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        #region Object results

        protected static ObjectResult Gone(object value)
        {
            return new ObjectResult(value)
            {
                StatusCode = StatusCodes.Status410Gone
            };
        }

        protected static ObjectResult InternalServerError(object value)
        {
            return new ObjectResult(value)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        #endregion
    }
}