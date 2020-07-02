using System.Threading.Tasks;
using Cashflow.Domain.DataTransferObjects;
using Cashflow.Domain.DomainObjects;

namespace Cashflow.Infrastructure.Repositories.Users
{
    public interface IUserRepository : IRepository<User, int>
    {
        bool UserWithEmailOrDisplayNameExist(string email, string displayName);
        Task<bool> UserWithEmailOrDisplayNameExistAsync(string email, string displayName);

        User GetByEmail(string email);
        Task<User> GetByEmailAsync(string email);

        UserTokenDto GetByAccessToken(string token);
        Task<UserTokenDto> GetByAccessTokenAsync(string token);
    }
}
