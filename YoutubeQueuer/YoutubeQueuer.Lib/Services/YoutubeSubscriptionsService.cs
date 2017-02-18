using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    internal class YoutubeSubscriptionsService : IYoutubeSubscriptionsService
    {
        public async Task<IEnumerable<YoutubeSubscriptionModel>> GetUserSubscriptions(UserCredential credential)
        {
            var youtube = new YouTubeService(new BaseClientService.Initializer
            {
                ApplicationName = "YtTestApp",
                HttpClientInitializer = credential
            });

            const string parts = "id,snippet,contentDetails";
            var channels = await GetChannels(credential, youtube, parts);
            var channelId = channels.Items.First().Id;

            var subs = await GetSubscriptions(credential, youtube, parts, channelId);
            return subs.Items.Select(x => new YoutubeSubscriptionModel
            {
                ChannelId = x.Snippet.ResourceId.ChannelId,
                ChannelName = x.Snippet.Title
            }).ToList();
        }

        private static async Task<SubscriptionListResponse> GetSubscriptions(UserCredential credential,
            YouTubeService youtube, string parts, string targetChannelId)
        {
            var subsRequest = youtube.Subscriptions.List(parts);

            subsRequest.ChannelId = targetChannelId;
            subsRequest.OauthToken = credential.Token.AccessToken;
            subsRequest.MaxResults = 50;

            var subs = await subsRequest.ExecuteAsync();
            return subs;
        }

        private static async Task<ChannelListResponse> GetChannels(UserCredential credential, YouTubeService youtube,
            string parts)
        {
            var channelsRequest = youtube.Channels.List(parts);
            channelsRequest.Mine = true;
            channelsRequest.OauthToken = credential.Token.AccessToken;

            var channels = await channelsRequest.ExecuteAsync();
            return channels;
        }
    }
}
