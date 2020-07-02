using System.Linq;
using System.Threading.Tasks;
using Cashflow.Domain.DataTransferObjects;
using Cashflow.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;

namespace Cashflow.Infrastructure.Repositories.Users
{
    public class UserRepository : Repository<User, int>, IUserRepository
    {
        public UserRepository(CashflowContext context) : base(context)
        {
        }

        public bool UserWithEmailOrDisplayNameExist(string email, string displayName)
        {
            return UserWithEmailOrDisplayNameExistAsync(email, displayName).Result;
        }
        public async Task<bool> UserWithEmailOrDisplayNameExistAsync(string email, string displayName)
        {
            return await Context.Users.AnyAsync(x => x.Email == email || x.DisplayName == displayName);
        }

        public User GetByEmail(string email)
        {
            return GetByEmailAsync(email).Result;
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return await Context.Users.FirstOrDefaultAsync(x => x.Email.Equals(email) && !x.DeletedDateTime.HasValue);
        }

        public UserTokenDto GetByAccessToken(string token)
        {
            return GetByAccessTokenAsync(token).Result;
        }
        public async Task<UserTokenDto> GetByAccessTokenAsync(string token)
        {
            return await Context.Users.Where(x => x.Token == token && !x.DeletedDateTime.HasValue).Select(x =>
            new UserTokenDto
            {
                UserId = x.Id,
                TokenExpirationDateTime = x.TokenExpirationDateTime
            }).FirstOrDefaultAsync();
        }
    }
}
