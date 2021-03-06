﻿using System;
using System.Collections.Generic;
using System.Configuration;
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

        private static readonly string SubscriptionsSettingsFileName =
            ConfigurationManager.AppSettings["SubscriptionSettingsFileName"];

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
            var data =
                _persistenceProvider.GetDataOrDefault<IEnumerable<UserSubscriptionSettingsModel>>(
                    SubscriptionsSettingsFileName);
            return data ?? Enumerable.Empty<UserSubscriptionSettingsModel>();
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
