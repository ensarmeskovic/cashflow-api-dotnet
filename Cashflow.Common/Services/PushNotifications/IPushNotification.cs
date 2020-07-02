using System.Threading.Tasks;

namespace Cashflow.Common.Services.PushNotifications
{
    public interface IPushNotification
    {
        Task<bool> SendAsync(string registeredUserDeviceToken, string title, string body);

        Task<bool> SendBulkAsync(string[] registeredUserDeviceTokens, string title, string body);
    }
}
