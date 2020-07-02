using System.Threading.Tasks;
using Cashflow.Core;
using Cashflow.Core.Configurations;

namespace Cashflow.Common.Services.Email
{
    public interface IEmailer
    {
        bool Send(string subject, string message, string senderEmail, string receiverEmail);
        Task<bool> SendAsync(string subject, string message, string senderEmail, string receiverEmail);

        bool Send(string subject, string message, SenderTypes senderType, string receiverEmail);
        Task<bool> SendAsync(string subject, string message, SenderTypes senderType, string receiverEmail);
    }
}
