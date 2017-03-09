using System.Collections.Generic;
using YoutubeQueuer.Common;
using YoutubeQueuer.Lib.Models;

namespace YoutubeQueuer.Lib.Services.Abstract
{
    public interface IUserSubscriptionsSettingsService
    {
        Result<IEnumerable<UserSubscriptionSettingsModel>> GetUserSubscriptionsSettings(string userName);

        Result SaveUserSubscriptionSettings(IEnumerable<UserSubscriptionSettingsModel> subscriptionSettings,
            string userName);
    }
}
