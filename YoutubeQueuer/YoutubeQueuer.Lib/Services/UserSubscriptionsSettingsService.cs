using System;
using System.Collections.Generic;
using YoutubeQueuer.Common;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    internal class UserSubscriptionsSettingsService : IUserSubscriptionsSettingsService
    {
        public Result<IEnumerable<UserSubscriptionSettingsModel>> GetUserSubscriptionsSettings(string userName)
        {
            throw new NotImplementedException();
        }

        public Result SaveUserSubscriptionSettings(IEnumerable<UserSubscriptionSettingsModel> subscriptionSettings,
            string userName)
        {
            throw new NotImplementedException();
        }
    }
}
