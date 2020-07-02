namespace Cashflow.Core.Configurations
{
    public class DatabaseConfiguration
    {
        public string ConnectionString { get; set; }
        public int ConnectionTimeout { get; set; } = 5;
    }
}
