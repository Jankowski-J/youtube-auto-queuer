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
        private string _token;
        private static UserCredential _credential;

        public async Task<string> GetAuthorizationTokenForApp(Stream serializedSettings, GoogleAuthorizationScope scope)
        {
            if (string.IsNullOrWhiteSpace(_token))
            {
                await AuthorizeApp(serializedSettings, scope);
            }

            return _token;
        }

        private async Task AuthorizeApp(Stream serializedSettings, GoogleAuthorizationScope scope)
        {
            var scopes = new List<string>();
            if (scope == GoogleAuthorizationScope.Youtube)
            {
                scopes.Add("https://www.googleapis.com/auth/youtube");
            }

            var credentials = GoogleCredential.FromStream(serializedSettings)
                .CreateScoped(scopes)
                .UnderlyingCredential as ServiceAccountCredential;
            _token = await credentials.GetAccessTokenForRequestAsync("https://www.googleapis.com/auth/youtube");
        }

        public async Task<IEnumerable<string>> GetUserSubscriptions(string userName)
        {
            var youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApplicationName = "YtTestApp",
                HttpClientInitializer = _credential
            });

            var parts = "id,snippet,contentDetails";
            var channelsRequest = youtube.Channels.List(parts);
            channelsRequest.Mine = true;
            channelsRequest.OauthToken = _token;

            var channels = await channelsRequest.ExecuteAsync();

            var subsRequest = youtube.Subscriptions.List(parts);

            subsRequest.ChannelId = channels.Items.First().Id;
            subsRequest.OauthToken = _token;
            subsRequest.MaxResults = 50;

            var subs = await subsRequest.ExecuteAsync();

            return subs.Items.Select(x => x.Snippet.ChannelId).ToList();
        }

        public async Task AuthorizeUser(string userName, Stream stream)
        {
            var secretAlt = GoogleClientSecrets.Load(stream).Secrets;

            _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(secretAlt
                , new List<string>
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