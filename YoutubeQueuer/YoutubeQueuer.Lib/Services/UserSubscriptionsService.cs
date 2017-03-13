using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using YoutubeQueuer.Common;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    internal class UserSubscriptionsService : IUserSubscriptionsService
    {
        private readonly IUserSubscriptionsSettingsService _subscriptionsSettingsService;
        private readonly IYoutubeSubscriptionsService _subscriptionsService;

        public UserSubscriptionsService(IUserSubscriptionsSettingsService subscriptionsSettingsService,
            IYoutubeSubscriptionsService subscriptionsService)
        {
            _subscriptionsSettingsService = subscriptionsSettingsService;
            _subscriptionsService = subscriptionsService;
        }

        public Result<IEnumerable<UserSubscriptionModel>> GetOnlyIncludedSubscriptions(UserCredential credential)
        {
            try
            {
                var settings = _subscriptionsSettingsService.GetUserSubscriptionsSettings(credential.UserId);

                var subscriptions = _subscriptionsService.GetUserSubscriptions(credential);

                if (!settings.IsSuccess || !subscriptions.IsSuccess)
                {
                    return Result<IEnumerable<UserSubscriptionModel>>.Fail();
                }

                var included = subscriptions.Data.Where(x =>
                {
                    var setting = settings.Data.FirstOrDefault(y => y.ChannelId == x.ChannelId);
                    return setting != null && setting.IsIncluded;
                }).Select(ToUserSubscriptionModel(credential));

                return Result<IEnumerable<UserSubscriptionModel>>.Succeed(included);
            }
            catch (Exception)
            {
                return Result<IEnumerable<UserSubscriptionModel>>.Fail();
            }
        }

        private static Func<YoutubeSubscriptionModel, UserSubscriptionModel> ToUserSubscriptionModel(
            UserCredential credential)
        {
            return x => new UserSubscriptionModel
            {
                ChannelId = x.ChannelId,
                ChannelName = x.ChannelName,
                IsIncluded = true,
                UserName = credential.UserId
            };
        }
    }
}
