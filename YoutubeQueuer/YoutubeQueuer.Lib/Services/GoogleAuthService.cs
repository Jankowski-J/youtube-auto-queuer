using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    internal class GoogleAuthService : IGoogleAuthService
    {
       public async Task<UserCredential> AuthorizeUser(Stream stream)
        {
            var secrets = GoogleClientSecrets.Load(stream).Secrets;

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                DataStore = new FileDataStore("C:\\Temp", true),
                Scopes = new List<string>
                {
                    YouTubeService.Scope.Youtube,
                    YouTubeService.Scope.YoutubeForceSsl
                }
            });

            var token = new TokenResponse { };
            var flow2 = new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = secrets,
                    Scopes = new List<string>
                    {
                        YouTubeService.Scope.Youtube,
                        YouTubeService.Scope.YoutubeForceSsl,
                    }
                });

            var app = new AuthorizationCodeInstalledApp(flow2, new LocalServerCodeReceiver());

            var task = GoogleWebAuthorizationBroker.AuthorizeAsync(secrets,
                new List<string>
                {
                    YouTubeService.Scope.Youtube,
                    YouTubeService.Scope.YoutubeForceSsl
                }, "user", CancellationToken.None,
                new Google.Apis.Util.Store.FileDataStore(GetType().ToString()), new LocalServerCodeReceiver())
                .ConfigureAwait(false);

            await task;
            return task.GetAwaiter().GetResult();
        }
    }
}