using System.Collections.Generic;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Providers.Abstract;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    internal class YoutubePlaylistsService : IYoutubePlaylistsService
    {
        private readonly IYoutubeServiceProvider _youtubeServiceProvider;

        public YoutubePlaylistsService(IYoutubeServiceProvider youtubeServiceProvider)
        {
            _youtubeServiceProvider = youtubeServiceProvider;
        }

        public IEnumerable<YoutubePlaylistModel> GetUserPlaylists(UserCredential credential)
        {
            var youtube = _youtubeServiceProvider.GetYoutubeService(credential);

            var request = youtube.Playlists.List("id,snippet,contentDetails");

            request.MaxResults = 50;
            request.Mine = true;

            var result = request.Execute();

            return result.Items.Select(x => new YoutubePlaylistModel
            {
                Name = x.Snippet.Title,
                Id = x.Id
            });
        }
    }
}
