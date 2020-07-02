using System.Collections.Generic;

namespace Cashflow.Core.Configurations
{
    public class EmailConfiguration
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }

        public int Port { get; set; }
        public bool EnableSsl { get; set; }

        public IEnumerable<EmailSender> Senders { get; set; }
    }

    public class EmailSender
    {
        public SenderTypes Type { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public enum SenderTypes
    {
        NoReply,
        Office,
        Marketing
    }
}
