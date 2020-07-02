using System.Collections.Generic;
using System.Threading.Tasks;
using Cashflow.Core;
using Cashflow.Core.Configurations;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace Cashflow.Common.Services.PushNotifications
{
    public class PushNotification : IPushNotification
    {
        private readonly FirebaseConfiguration _firebaseConfiguration;
        private readonly FirebaseMessaging _firebaseMessaging;

        public PushNotification(FirebaseConfiguration firebaseConfiguration, FirebaseMessaging firebaseMessaging)
        {
            _firebaseConfiguration = firebaseConfiguration;

            FirebaseApp app = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("cashflow-firebase-adminsdk.json")
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging")
            });

            _firebaseMessaging = FirebaseMessaging.GetMessaging(app);
        }

        #region Send
        public async Task<bool> SendAsync(string registeredUserDeviceToken, string title, string body)
        {
            string result = await _firebaseMessaging.SendAsync(Notification(registeredUserDeviceToken, title, body));

            //test
            return true;
        }

        public async Task<bool> SendBulkAsync(string[] registeredUserDeviceTokens, string title, string body)
        {
            BatchResponse result = await _firebaseMessaging.SendMulticastAsync(Notification(registeredUserDeviceTokens, title, body));

            return result.SuccessCount > 0;
        }
        #endregion

        #region Helpers

        private static Message Notification(string token, string title, string body)
        {
            return new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Body = body,
                    Title = title
                }
            };
        }

        private static MulticastMessage Notification(IReadOnlyList<string> tokens, string title, string body)
        {
            return new MulticastMessage
            {
                Tokens = tokens,
                Notification = new Notification
                {
                    Body = body,
                    Title = title
                }
            };
        }

        #endregion
    }
}
