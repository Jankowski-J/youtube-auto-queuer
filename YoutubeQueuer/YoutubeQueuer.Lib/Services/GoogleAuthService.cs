using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private static UserCredential _credential;

        public async Task<IEnumerable<string>> GetUserSubscriptions(string userName)
        {
            var youtube = new YouTubeService(new BaseClientService.Initializer
            {
                ApplicationName = "YtTestApp",
                HttpClientInitializer = _credential
            });

            const string parts = "id,snippet,contentDetails";
            var channelsRequest = youtube.Channels.List(parts);
            channelsRequest.Mine = true;
            channelsRequest.OauthToken = _credential.Token.AccessToken;

            var channels = await channelsRequest.ExecuteAsync();

            var subsRequest = youtube.Subscriptions.List(parts);

            subsRequest.ChannelId = channels.Items.First().Id;
            subsRequest.OauthToken = _credential.Token.AccessToken;
            subsRequest.MaxResults = 50;

            var subs = await subsRequest.ExecuteAsync();

            return subs.Items.Select(x => x.Snippet.ChannelId).ToList();
        }

        public async Task AuthorizeUser(string userName, Stream stream)
        {
            var secrets = GoogleClientSecrets.Load(stream).Secrets;

            _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets,
                new List<string>
                {
                    YouTubeService.Scope.Youtube,
                    YouTubeService.Scope.YoutubeForceSsl
                }, "user", CancellationToken.None,
                new Google.Apis.Util.Store.FileDataStore(GetType().ToString()));
        }

        public string GetAuthorizedUserId()
        {
            return _credential.UserId;
        }
    }
}