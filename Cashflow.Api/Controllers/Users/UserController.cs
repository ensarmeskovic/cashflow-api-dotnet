using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cashflow.Api.Controllers.Users.Requests;
using Cashflow.Api.Controllers.Users.Responses;
using Cashflow.Api.Filters;
using Cashflow.Common.Services.Cryptography;
using Cashflow.Common.Services.Email;
using Cashflow.Common.Services.RandomGenerator;
using Cashflow.Common.Services.TokenProcessor;
using Cashflow.Core.Configurations;
using Cashflow.Domain.DomainObjects;
using Cashflow.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace Cashflow.Api.Controllers.Users
{
    [ApiController, Route("[controller]")]
    public class UserController : BaseController
    {
        private readonly ICryptography _cryptography;
        private readonly IRandomGenerator _randomGenerator;
        private readonly ITokenProcessor _tokenProcessor;
        private readonly IEmailer _emailer;

        private readonly TokenConfiguration _tokenConfiguration;

        public UserController(IUnitOfWork unitOfWork, ICryptography cryptography,
            ITokenProcessor tokenProcessor, TokenConfiguration tokenConfiguration, IRandomGenerator randomGenerator, IEmailer emailer) : base(unitOfWork)
        {
            _cryptography = cryptography;
            _tokenProcessor = tokenProcessor;
            _randomGenerator = randomGenerator;
            _emailer = emailer;

            _tokenConfiguration = tokenConfiguration;
        }

        #region Registration

        [HttpPost("registration")]
        public async Task<ActionResult> Registration([FromBody] RegistrationRequest request)
        {
            try
            {
                if (!request.Password.Equals(request.PasswordConfirmation))
                    return BadRequest("Passwords do not match!");

                Regex pattern = new Regex("(?=.*[0-9])(?=.*[a-zA-Z]).");
                if (!pattern.IsMatch(request.Password) || request.Password.Length < 6)
                    return BadRequest("Password must contain both letters and numbers and minimum of 6 characters!");

                bool emailOrDisplayNameExist = await UnitOfWork.UserRepository.UserWithEmailOrDisplayNameExistAsync(request.Email, request.DisplayName);
                if (emailOrDisplayNameExist)
                    return BadRequest("User with the same email or display name already exists!");

                string salt = _cryptography.CreateSalt();
                string hash = _cryptography.CreateHash(request.Password, salt);

                User user = new User
                {
                    DisplayName = request.DisplayName,
                    Email = request.Email,
                    PasswordSalt = salt,
                    PasswordHash = hash,
                    RegistrationConfirmed = false,
                    RegistrationCode = _randomGenerator.GenerateRandomAlphanumericString(6).ToUpper()
                };

                await UnitOfWork.ExecuteTransactionAsync(async (transaction, timeout) =>
                {
                    UnitOfWork.UserRepository.Add(user);
                    await UnitOfWork.SaveChangesAsync();
                }, async () =>
                {
                    string baseUrl = $"{Request.Scheme}://{Request.Host}/{nameof(UserController).Remove("Controller")}/{nameof(RegistrationCodeConfirmation).ToLower()}/";
                    string encryptedRegistrationCode = _cryptography.Encrypt($"#{user.Id}#{user.RegistrationCode}");

                    await _emailer.SendAsync("Registration code confirmation", $"{baseUrl}{encryptedRegistrationCode}", SenderTypes.NoReply, user.Email);
                }, null);
                
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex.InnerExceptionMessage());
            }
        }

        [HttpGet("registrationcodeconfirmation")]
        public async Task<ActionResult> RegistrationCodeConfirmation([FromQuery] string id)
        {
            string registrationToken = _cryptography.Decrypt(id);

            try
            {
                if (!registrationToken.IsSet())
                    return BadRequest("Code is missing!");

                int? userId;
                string registrationCode;

                try
                {
                    List<string> codeKeys = registrationToken.Split("#").ToList();
                    codeKeys.RemoveAll(x => !x.IsSet());

                    userId = codeKeys.Count != 0
                        ? int.Parse(codeKeys.FirstOrDefault())
                        : throw new ArgumentNullException();
                    registrationCode = codeKeys.LastOrDefault();
                }
                catch
                {
                    return BadRequest("Invalid link!");
                }

                User user = await UnitOfWork.UserRepository.GetByIdAsync(userId.GetValueOrDefault());
                if (!user.RegistrationCode.Equals(registrationCode))
                    return BadRequest("Invalid registration code!");

                await UnitOfWork.ExecuteTransactionAsync(async (transaction, timeout) =>
                {
                    user.RegistrationConfirmed = true;
                    UnitOfWork.UserRepository.Update(user);

                    Inbox inbox = new Inbox
                    {
                        Name = "Personal",
                        Active = true,
                        AdminId = user.Id
                    };
                    UnitOfWork.InboxRepository.Add(inbox);
                    
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

        #region Login

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
        {
            User user = await UnitOfWork.UserRepository.GetByEmailAsync(request.Email);
            if (user is null || !_cryptography.Validate(request.Password, user.PasswordSalt, user.PasswordHash))
                return Unauthorized("Incorrect username and/or password.");

            await UnitOfWork.ExecuteTransactionAsync(async (transaction, timeout) =>
            {
                user.DeviceToken = request.DeviceToken;
                user.Token = _tokenProcessor.GenerateToken(user.GetUserClaims());
                user.TokenExpirationDateTime = DateTime.Now.AddMinutes(_tokenConfiguration.TokenExpiresInMinutes);

                UnitOfWork.UserRepository.Update(user);
                await UnitOfWork.SaveChangesAsync();
            }, null, null);

            return Ok(new TokenResponse(user.Token, _tokenConfiguration.TokenExpiresInMinutes));
        }

        #endregion

        #region Logout

        [HttpGet("logout"), TokenValidation]
        public async Task<ActionResult> Logout()
        {
            LoggedUser loggedUser = LoggedUser;

            try
            {
                User user = await UnitOfWork.UserRepository.GetByIdAsync(loggedUser.UserId);

                await UnitOfWork.ExecuteTransactionAsync(async (transaction, timeout) =>
                {
                    user.DeviceToken = null;
                    user.Token = null;
                    user.TokenExpirationDateTime = null;

                    UnitOfWork.UserRepository.Update(user);
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

        #region Change password

        [HttpPost("change-password"), TokenValidation]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            LoggedUser loggedUser = LoggedUser;

            try
            {
                if (!request.Password.Equals(request.PasswordConfirmation))
                    return BadRequest("Passwords do not match!");

                if (request.OldPassword.Equals(request.Password))
                    return BadRequest("You can not set old password!");

                Regex pattern = new Regex("(?=.*[0-9])(?=.*[a-zA-Z]).");
                if (!pattern.IsMatch(request.Password) || request.Password.Length < 6)
                    return BadRequest("Password must contain both letters and numbers and minimum of 6 characters!");

                User user = await UnitOfWork.UserRepository.GetByIdAsync(loggedUser.UserId);
                if (!_cryptography.Validate(request.OldPassword, user.PasswordSalt, user.PasswordHash))
                    return BadRequest("Please enter valid old password");

                await UnitOfWork.ExecuteTransactionAsync(async (transaction, timeout) =>
                {
                    user.PasswordSalt = _cryptography.CreateSalt();
                    user.PasswordHash = _cryptography.CreateHash(request.Password, user.PasswordSalt);

                    UnitOfWork.UserRepository.Update(user);
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

    }
}