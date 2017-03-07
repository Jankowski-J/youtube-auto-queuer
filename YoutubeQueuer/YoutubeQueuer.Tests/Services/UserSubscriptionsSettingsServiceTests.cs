using System.Collections.Generic;
using NUnit.Framework;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Services;

namespace YoutubeQueuer.Tests.Services
{
    [TestFixture]
    public class UserSubscriptionsSettingsServiceTests
    {
        private UserSubscriptionsSettingsService _target;

        [SetUp]
        public void Initialize()
        {
            _target = new UserSubscriptionsSettingsService();
        }

        [Test]
        public void SaveUserSubscriptionSettings_For1IncludedAnd2Not_ShouldSucceed()
        {
            // Arrange
            const string userName = "andrzej";
            var subscriptions = new List<UserSubscriptionSettingsModel>
            {
                new UserSubscriptionSettingsModel
                {
                    ChannelId = "aaa",
                    IsIncluded = true
                },
                new UserSubscriptionSettingsModel
                {
                    ChannelId = "bbb",
                    IsIncluded = false
                },
                new UserSubscriptionSettingsModel
                {
                    ChannelId = "ccc",
                    IsIncluded = false
                }
            };

            // Act
            var result = _target.SaveUserSubscriptionSettings(subscriptions, userName);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }
    }
}
