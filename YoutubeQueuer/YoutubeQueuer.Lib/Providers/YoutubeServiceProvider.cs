using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using YoutubeQueuer.Lib.Providers.Abstract;

namespace YoutubeQueuer.Lib.Providers
{
    internal class YoutubeServiceProvider : IYoutubeServiceProvider
    {
        private const string AppName = "YoutubeVideoQueuer";

        public YouTubeService GetYoutubeService(UserCredential credential)
        {
            return new YouTubeService(new BaseClientService.Initializer
            {
                ApplicationName = AppName,
                HttpClientInitializer = credential
            });
        }
    }
}
