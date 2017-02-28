using System.Collections.Generic;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Providers.Abstract;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    internal class YoutubeSubscriptionsService : IYoutubeSubscriptionsService
    {
        private readonly IYoutubeServiceProvider _youtubeServiceProvider;

        public YoutubeSubscriptionsService(IYoutubeServiceProvider youtubeServiceProvider)
        {
            _youtubeServiceProvider = youtubeServiceProvider;
        }

        public IEnumerable<YoutubeSubscriptionModel> GetUserSubscriptions(UserCredential credential)
        {
            var youtube = _youtubeServiceProvider.GetYoutubeService(credential);

            const string parts = "id,snippet,contentDetails";
            var channels = GetChannels(credential, youtube, parts);
            var channelId = channels.Items.First().Id;

            var subs = GetSubscriptions(credential, youtube, parts, channelId);
            return subs.Items.Select(x => new YoutubeSubscriptionModel
            {
                ChannelId = x.Snippet.ResourceId.ChannelId,
                ChannelName = x.Snippet.Title
            }).ToList();
        }

        private static SubscriptionListResponse GetSubscriptions(UserCredential credential,
            YouTubeService youtube, string parts, string targetChannelId)
        {
            var subsRequest = youtube.Subscriptions.List(parts);

            subsRequest.ChannelId = targetChannelId;
            subsRequest.OauthToken = credential.Token.AccessToken;
            subsRequest.MaxResults = 50;

            var subs = subsRequest.Execute();
            return subs;
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
