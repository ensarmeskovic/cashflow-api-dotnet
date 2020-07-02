using System.Collections.Generic;
using System.Security.Claims;

namespace Cashflow.Common.Services.TokenProcessor
{
    public interface ITokenProcessor
    {
        string GenerateToken(Claim[] claims);
        bool IsValidToken(string token);
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
