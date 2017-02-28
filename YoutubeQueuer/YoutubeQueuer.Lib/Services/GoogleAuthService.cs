using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    internal class GoogleAuthService : IGoogleAuthService
    {
       public async Task<UserCredential> AuthorizeUser(Stream stream)
        {
            var secrets = GoogleClientSecrets.Load(stream).Secrets;

            var task = GoogleWebAuthorizationBroker.AuthorizeAsync(secrets,
                new List<string>
                {
                    YouTubeService.Scope.Youtube,
                    YouTubeService.Scope.YoutubeForceSsl
                }, "user", CancellationToken.None,
                new Google.Apis.Util.Store.FileDataStore(GetType().ToString()), new LocalServerCodeReceiver());
            return await task;
        }
    }
}