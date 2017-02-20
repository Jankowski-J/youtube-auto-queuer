using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;

namespace YoutubeQueuer.Lib.Providers.Abstract
{
    public interface IYoutubeServiceProvider
    {
        YouTubeService GetYoutubeService(UserCredential credential);
    }
}
