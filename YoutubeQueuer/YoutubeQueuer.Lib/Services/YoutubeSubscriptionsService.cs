using System.Collections.Generic;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using YoutubeQueuer.Common;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Providers.Abstract;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    internal class YoutubeSubscriptionsService : IYoutubeSubscriptionsService
    {
        private readonly IYoutubeServiceProvider _youtubeServiceProvider;
        private readonly IYoutubeConstsProvider _youtubeConstsProvider;

        public YoutubeSubscriptionsService(IYoutubeServiceProvider youtubeServiceProvider,
            IYoutubeConstsProvider youtubeConstsProvider)
        {
            _youtubeServiceProvider = youtubeServiceProvider;
            _youtubeConstsProvider = youtubeConstsProvider;
        }

        public Result<IEnumerable<YoutubeSubscriptionModel>> GetUserSubscriptions(UserCredential credential)
        {
            var youtube = _youtubeServiceProvider.GetYoutubeService(credential);
            var channels = GetChannels(credential, youtube, _youtubeConstsProvider.ChannelsListParts);
            var channelId = channels.Items.First().Id;

            var subs = GetAllUserSubscriptions(credential, youtube, _youtubeConstsProvider.SubscriptionsListParts, channelId);
            return Result<IEnumerable<YoutubeSubscriptionModel>>.Succeed(subs.ToList());
        }

        private IEnumerable<YoutubeSubscriptionModel> GetAllUserSubscriptions(UserCredential credential,
            YouTubeService youtube, string parts, string targetChannelId)
        {
            var subsRequest = CreateSubscriptionsRequest(credential, youtube, parts, targetChannelId);

            var subs = subsRequest.Execute();
            var nextPageToken = subs.NextPageToken;
            var allSubscriptions = subs.Items.Select(ToYoutubeSubscriptionModel).ToList();

            while (!string.IsNullOrWhiteSpace(nextPageToken))
            {
                var nextPageRequest = CreateSubscriptionsRequest(credential, youtube, parts, targetChannelId);
                nextPageRequest.PageToken = nextPageToken;

                var result = nextPageRequest.Execute();
                allSubscriptions.AddRange(result.Items.Select(ToYoutubeSubscriptionModel));
                nextPageToken = result.NextPageToken;
            }

            return allSubscriptions;
        }

        private SubscriptionsResource.ListRequest CreateSubscriptionsRequest(UserCredential credential,
            YouTubeService youtube, string parts, string targetChannelId)
        {
            var subsRequest = youtube.Subscriptions.List(parts);

            subsRequest.ChannelId = targetChannelId;
            subsRequest.OauthToken = credential.Token.AccessToken;
            subsRequest.MaxResults = _youtubeConstsProvider.MaxResultsCount;
            return subsRequest;
        }

        private static YoutubeSubscriptionModel ToYoutubeSubscriptionModel(Subscription subscription)
        {
            return new YoutubeSubscriptionModel
            {
                ChannelId = subscription.Snippet.ResourceId.ChannelId,
                ChannelName = subscription.Snippet.Title
            };
        }

        private static ChannelListResponse GetChannels(UserCredential credential, YouTubeService youtube,
            string parts)
        {
            var channelsRequest = youtube.Channels.List(parts);
            channelsRequest.Mine = true;
            channelsRequest.OauthToken = credential.Token.AccessToken;

            var channels = channelsRequest.Execute();
            return channels;
        }
    }
}
