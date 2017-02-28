using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3.Data;
using YoutubeQueuer.Common;
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

        public Result AddVideosToPlaylist(IEnumerable<string> videoIds, string playlistId, UserCredential credential)
        {
            try
            {
                var youtube = _youtubeServiceProvider.GetYoutubeService(credential);

                var items = videoIds
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => new PlaylistItem
                    {
                        Id = playlistId,

                        Snippet = new PlaylistItemSnippet
                        {
                            PlaylistId = playlistId,
                            ResourceId = new ResourceId
                            {
                                VideoId = x,
                                Kind = "youtube#video",
                                PlaylistId = playlistId
                            }
                        },
                        Kind = "youtube#playlistIem"
                    }).ToList();

                items.AsParallel().ForAll(x =>
                {
                    var req = youtube.PlaylistItems.Insert(x, "id,snippet");
                    req.Execute();
                });
                return Result.Succeed();
            }
            catch (Exception e)
            {
                return Result.Fail();
            }
        }
    }
}
