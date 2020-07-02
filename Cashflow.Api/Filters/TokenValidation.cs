using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using Cashflow.Common.Services.TokenProcessor;
using Cashflow.Domain.DataTransferObjects;
using Cashflow.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cashflow.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TokenValidation : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            IUnitOfWork unitOfWork = (IUnitOfWork)context.HttpContext.RequestServices.GetService(typeof(IUnitOfWork));
            ITokenProcessor tokenProcessor = (ITokenProcessor)context.HttpContext.RequestServices.GetService(typeof(ITokenProcessor));

            string token = context.HttpContext.Request.Headers["authentication"];

            if (!token.IsSet())
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Result =
                    new JsonResult("Access token is not sent with the request. Check your request headers!");
            }
            else
            {
                bool addClaimsToHttpContext = true;
                //ToDo :: encrypt whole jwt token with hardcoded key
                UserTokenDto user = unitOfWork.UserRepository.GetByAccessToken(token);

                if (user == null)
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Result = new JsonResult("User doesn't exists. Check Access Token!");
                    addClaimsToHttpContext = false;
                }
                else if (DateTime.Now > user.TokenExpirationDateTime)
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Result = new JsonResult("Bad access token!");
                    addClaimsToHttpContext = false;
                }

                if (addClaimsToHttpContext)
                {
                    IEnumerable<Claim> claims = tokenProcessor.GetTokenClaims(token);
                    if (claims != null)
                    {
                        ClaimsIdentity appIdentity = new ClaimsIdentity(claims);
                        context.HttpContext.User.AddIdentity(appIdentity);
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}