using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    public class GoogleAuthService : IGoogleAuthService
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

            var subs = await GetSubscriptions(credential, youtube, parts, channels);
            return subs.Items.Select(x => new YoutubeSubscriptionModel
            {
                ChannelId = x.Snippet.ChannelId,
                ChannelName = x.Snippet.ChannelTitle
            }).ToList();
        }

        private static async Task<SubscriptionListResponse> GetSubscriptions(UserCredential credential, YouTubeService youtube, string parts,
            ChannelListResponse channels)
        {
            var subsRequest = youtube.Subscriptions.List(parts);

            subsRequest.ChannelId = channels.Items.First().Id;
            subsRequest.OauthToken = credential.Token.AccessToken;
            subsRequest.MaxResults = 50;

            var subs = await subsRequest.ExecuteAsync();
            return subs;
        }

        private static async Task<ChannelListResponse> GetChannels(UserCredential credential, YouTubeService youtube, string parts)
        {
            var channelsRequest = youtube.Channels.List(parts);
            channelsRequest.Mine = true;
            channelsRequest.OauthToken = credential.Token.AccessToken;

            var channels = await channelsRequest.ExecuteAsync();
            return channels;
        }

        public async Task<UserCredential> AuthorizeUser(Stream stream)
        {
            var secrets = GoogleClientSecrets.Load(stream).Secrets;

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets,
                new List<string>
                {
                    YouTubeService.Scope.Youtube,
                    YouTubeService.Scope.YoutubeForceSsl
                }, "user", CancellationToken.None,
                new Google.Apis.Util.Store.FileDataStore(GetType().ToString()));
        }

        //public string GetAuthorizedUserId()
        //{
        //    return _credential.UserId;
        //}
    }
}