namespace Cashflow.Core.Configurations
{
    public class TokenConfiguration
    {
        public  string Issuer { get; set; }
        public  string Audience { get; set; }
        public  string SecurityString { get; set; }

        public  int TokenExpiresInMinutes { get; set; }
        //public int RefreshExpiresInMinutes { get; set; }
    }
}
