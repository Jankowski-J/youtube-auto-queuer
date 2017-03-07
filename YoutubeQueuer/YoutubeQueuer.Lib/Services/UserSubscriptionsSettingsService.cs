using System;
using System.Collections.Generic;
using System.Linq;
using YoutubeQueuer.Common;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Providers.Abstract;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    internal class UserSubscriptionsSettingsService : IUserSubscriptionsSettingsService
    {
        private readonly IFileSystemPersistenceProvider _persistenceProvider;
        private readonly string SubscriptionsSettingsFileName = "subscription_settings.json";

        public UserSubscriptionsSettingsService(IFileSystemPersistenceProvider persistenceProvider)
        {
            _persistenceProvider = persistenceProvider;
        }

        public Result<IEnumerable<UserSubscriptionSettingsModel>> GetUserSubscriptionsSettings(string userName)
        {
            try
            {
                var settings = GetAllSubscriptionsSettings();
                var filtered = settings.Where(x => x.UserName == userName);

                return Result<IEnumerable<UserSubscriptionSettingsModel>>.Succeed(filtered);
            }
            catch (Exception)
            {
                return Result<IEnumerable<UserSubscriptionSettingsModel>>.Fail();
            }
        }

        private IEnumerable<UserSubscriptionSettingsModel> GetAllSubscriptionsSettings()
        {
            return _persistenceProvider.GetData<IEnumerable<UserSubscriptionSettingsModel>>(SubscriptionsSettingsFileName);
        }

        public Result SaveUserSubscriptionSettings(IEnumerable<UserSubscriptionSettingsModel> subscriptionSettings,
            string userName)
        {
            try
            {
                var currentData = GetAllSubscriptionsSettings()
                    .Where(x => x.UserName != userName);

                var updatedData = currentData.Concat(subscriptionSettings).ToList();
                _persistenceProvider.PersistData(updatedData, SubscriptionsSettingsFileName);

                return Result.Succeed();
            }
            catch (Exception)
            {
                return Result.Fail();
            }
        }
    }
}
