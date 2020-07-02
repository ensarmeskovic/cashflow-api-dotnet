namespace Cashflow.Api.Controllers.Users.Responses
{
    public class TokenResponse
    {
        public TokenResponse(string token, int tokenExpiresInMinutes)
        {
            Token = token;
            TokenExpiresInMinutes = tokenExpiresInMinutes;
        }

        public string Token { get; set; }
        public int TokenExpiresInMinutes { get; set; }
    }
}
