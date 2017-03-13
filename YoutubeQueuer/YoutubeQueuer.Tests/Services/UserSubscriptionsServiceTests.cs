using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Moq;
using Ploeh.AutoFixture;
using Xunit;
using YoutubeQueuer.Common;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Services;
using YoutubeQueuer.Lib.Services.Abstract;
using YoutubeQueuer.Tests.MockData;

namespace YoutubeQueuer.Tests.Services
{
    public class UserSubscriptionsServiceTests
    {
        private readonly UserSubscriptionsService _target;
        private readonly Mock<IUserSubscriptionsSettingsService> _subscriptionsSettingsService;
        private readonly Mock<IYoutubeSubscriptionsService> _subscriptionsService;
        private readonly IFixture _fixture;

        public UserSubscriptionsServiceTests()
        {
            _subscriptionsSettingsService = new Mock<IUserSubscriptionsSettingsService>();
            _subscriptionsService = new Mock<IYoutubeSubscriptionsService>();
            _target = new UserSubscriptionsService(_subscriptionsSettingsService.Object, _subscriptionsService.Object);
            _fixture = new Fixture();

            _fixture.Register<IAuthorizationCodeFlow>(() => _fixture.Create<MockAuthorizationCodeFlow>());
        }

        [Fact]
        public void GetOnlyIncludedVideos_ForNotWorkingSettingsService_ShouldFail()
        {
            // Arrange
            _subscriptionsSettingsService.Setup(x => x.GetUserSubscriptionsSettings(It.IsAny<string>()))
                .Throws(new Exception());
            var credential = _fixture.Create<UserCredential>();

            // Act
            var result = _target.GetOnlyIncludedSubscriptions(credential);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void GetOnlyIncludedVideos_ForNotWorkingSubscriptionsService_ShouldFail()
        {
            // Arrange
            _subscriptionsSettingsService.Setup(x => x.GetUserSubscriptionsSettings(It.IsAny<string>()))
                .Returns(Result<IEnumerable<UserSubscriptionSettingsModel>>.Succeed(
                    new List<UserSubscriptionSettingsModel>()));

            _subscriptionsService.Setup(x => x.GetUserSubscriptions(It.IsAny<UserCredential>()))
                .Throws(new Exception());

            var credential = _fixture.Create<UserCredential>();

            // Act
            var result = _target.GetOnlyIncludedSubscriptions(credential);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void GetOnlyIncludedVideos_ForOneIncludedAndOneExcludedVideo_ShouldSucceed()
        {
            // Arrange
            _subscriptionsSettingsService.Setup(x => x.GetUserSubscriptionsSettings(It.IsAny<string>()))
                .Returns(Result<IEnumerable<UserSubscriptionSettingsModel>>.Succeed(
                    new List<UserSubscriptionSettingsModel>
                    {
                        new UserSubscriptionSettingsModel
                        {
                            IsIncluded = true,
                            ChannelId = "a"
                        },
                        new UserSubscriptionSettingsModel
                        {
                            IsIncluded = false,
                            ChannelId = "b"
                        }
                    }));

            _subscriptionsService.Setup(x => x.GetUserSubscriptions(It.IsAny<UserCredential>()))
                .Returns(Result<IEnumerable<YoutubeSubscriptionModel>>.Succeed(new List<YoutubeSubscriptionModel>
                {
                    new YoutubeSubscriptionModel
                    {
                        ChannelId = "a"
                    },
                    new UserSubscriptionModel
                    {
                        ChannelId = "b"
                    }
                }));

            var credential = _fixture.Create<UserCredential>();

            // Act
            var result = _target.GetOnlyIncludedSubscriptions(credential);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void GetOnlyIncludedVideos_ForOneIncludedAndOneExcludedVideo_ShouldReturnOnlyIncludedOne()
        {
            // Arrange
            _subscriptionsSettingsService.Setup(x => x.GetUserSubscriptionsSettings(It.IsAny<string>()))
                .Returns(Result<IEnumerable<UserSubscriptionSettingsModel>>.Succeed(
                    new List<UserSubscriptionSettingsModel>
                    {
                        new UserSubscriptionSettingsModel
                        {
                            IsIncluded = true,
                            ChannelId = "a"
                        },
                        new UserSubscriptionSettingsModel
                        {
                            IsIncluded = false,
                            ChannelId = "b"
                        }
                    }));

            _subscriptionsService.Setup(x => x.GetUserSubscriptions(It.IsAny<UserCredential>()))
                .Returns(Result<IEnumerable<YoutubeSubscriptionModel>>.Succeed(new List<YoutubeSubscriptionModel>
                {
                    new YoutubeSubscriptionModel
                    {
                        ChannelId = "a"
                    },
                    new YoutubeSubscriptionModel
                    {
                        ChannelId = "b"
                    }
                }));

            var credential = _fixture.Create<UserCredential>();

            // Act
            var result = _target.GetOnlyIncludedSubscriptions(credential);

            // Assert
            Assert.Equal(1, result.Data.Count(x => x.ChannelId == "a" && x.IsIncluded));
            Assert.Equal(0, result.Data.Count(x => x.ChannelId == "b" && !x.IsIncluded));
        }
    }
}
