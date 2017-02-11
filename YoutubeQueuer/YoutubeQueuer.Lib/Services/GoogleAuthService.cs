using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private string _token;

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
            var youtubeService = new YouTubeService();

            var parts = "id,snippet";
            var channelsRequest = youtubeService.Channels.List(parts);
            channelsRequest.Mine = true;
            channelsRequest.OauthToken = _token;

            var channels = await channelsRequest.ExecuteAsync();

            var subsRequest = youtubeService.Subscriptions.List(parts);
            
            subsRequest.ChannelId = channels.Items.First().Id;
            subsRequest.OauthToken = _token;

            var subs = await subsRequest.ExecuteAsync();

            return subs.Items.Select(x => x.Snippet.ChannelId).ToList();
        }
    }
}