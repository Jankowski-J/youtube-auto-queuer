using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Providers.Abstract;
using YoutubeQueuer.Lib.Services;

namespace YoutubeQueuer.Tests.Services
{
    public class UserSubscriptionsSettingsServiceTests
    {
        private UserSubscriptionsSettingsService _target;
        private Mock<IFileSystemPersistenceProvider> _persistenceProvider;

        public UserSubscriptionsSettingsServiceTests()
        {
            Setup();
        }

        public void Setup()
        {
            _persistenceProvider = new Mock<IFileSystemPersistenceProvider>();
            _target = new UserSubscriptionsSettingsService(_persistenceProvider.Object);
        }

        [Fact]
        public void SaveUserSubscriptionSettings_For1IncludedAnd2Not_ShouldSucceed()
        {
            // Arrange
            const string userName = "andrzej";
            var subscriptions = new List<UserSubscriptionSettingsModel>
            {
                new UserSubscriptionSettingsModel
                {
                    ChannelId = "aaa",
                    IsIncluded = true,
                    UserName = userName
                },
                new UserSubscriptionSettingsModel
                {
                    ChannelId = "bbb",
                    IsIncluded = false,
                    UserName = userName
                },
                new UserSubscriptionSettingsModel
                {
                    ChannelId = "ccc",
                    IsIncluded = false,
                    UserName = userName
                }
            };

            // Act
            var result = _target.SaveUserSubscriptionSettings(subscriptions, userName);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void GetUserSubscriptionsSettings_ProvidedSettingsHaveManyUsers_ShouldReturnOnlyForSpecifiedUser()
        {
            // Arrange
            const string userName = "andrzej";
            var subscriptions = GetSubscriptionsForMoreThanOneUser(userName);

            _persistenceProvider.Setup(x => x.GetDataOrDefault<IEnumerable<UserSubscriptionSettingsModel>>(It.IsAny<string>()))
                .Returns(subscriptions);

            // Act
            var result = _target.GetUserSubscriptionsSettings(userName);

            // Assert
            Assert.Equal(2, result.Data.Count());
            Assert.True(result.Data.All(x => x.UserName == userName));
        }

        [Fact]
        public void GetUserSubscriptionsSettings_ProvidedSettingsHaveManyUsers_ShouldSucceed()
        {
            // Arrange
            const string userName = "andrzej";
            var subscriptions = GetSubscriptionsForMoreThanOneUser(userName);

            _persistenceProvider.Setup(x => x.GetDataOrDefault<IEnumerable<UserSubscriptionSettingsModel>>(It.IsAny<string>()))
                .Returns(subscriptions);

            // Act
            var result = _target.GetUserSubscriptionsSettings(userName);

            // Assert
            Assert.Equal(2, result.Data.Count());
            Assert.True(result.IsSuccess);
        }

        private static IEnumerable<UserSubscriptionSettingsModel> GetSubscriptionsForMoreThanOneUser(string userName)
        {
            var subscriptions = new List<UserSubscriptionSettingsModel>
            {
                new UserSubscriptionSettingsModel
                {
                    ChannelId = "aaa",
                    IsIncluded = true,
                    UserName = userName
                },
                new UserSubscriptionSettingsModel
                {
                    ChannelId = "bbb",
                    IsIncluded = false,
                    UserName = userName
                },
                new UserSubscriptionSettingsModel
                {
                    ChannelId = "ccc",
                    IsIncluded = false,
                    UserName = userName + "aaaaaaaaa"
                }
            };
            return subscriptions;
        }
    }
}
