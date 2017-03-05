using System;
using System.Collections.Generic;
using System.Linq;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
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
        private readonly IYoutubeConstsProvider _youtubeConstsProvider;

        public YoutubePlaylistsService(IYoutubeServiceProvider youtubeServiceProvider,
            IYoutubeConstsProvider youtubeConstsProvider)
        {
            _youtubeServiceProvider = youtubeServiceProvider;
            _youtubeConstsProvider = youtubeConstsProvider;
        }

        public IEnumerable<YoutubePlaylistModel> GetUserPlaylists(UserCredential credential)
        {
            var youtube = _youtubeServiceProvider.GetYoutubeService(credential);

            var request = youtube.Playlists.List(_youtubeConstsProvider.PlaylistsListParts);

            request.MaxResults = 50;
            request.Mine = true;

            var result = request.Execute();

            var chann = youtube.Channels.List("id,snippet,contentDetails");
            chann.Mine = true;

            var playlists = MapPlaylists(result);

            return playlists;
        }

        private static IEnumerable<YoutubePlaylistModel> MapPlaylists(PlaylistListResponse result)
        {
            var playlists = result.Items.Select(x => new YoutubePlaylistModel
            {
                Name = x.Snippet.Title,
                Id = x.Id
            }).ToList();

            playlists.Add(new YoutubePlaylistModel
            {
                Id = "WL",
                Name = "Watch later"
            });
            return playlists;
        }

        public Result AddVideosToPlaylist(IEnumerable<string> videoIds, string playlistId, UserCredential credential)
        {
            try
            {
                var youtube = _youtubeServiceProvider.GetYoutubeService(credential);

                var items = videoIds
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => ToPlaylistItem(playlistId, x)).ToList();

                foreach (var playlistItem in items)
                {
                    AddVideoToPlaylist(youtube, playlistItem);
                }
                return Result.Succeed();
            }
            catch (Exception e)
            {
                return Result.Fail();
            }
        }

        private void AddVideoToPlaylist(YouTubeService youtube, PlaylistItem playlistItem)
        {
            try
            {
                var req = youtube.PlaylistItems.Insert(playlistItem, _youtubeConstsProvider.InsertPlaylistItemsParts);
                req.Execute();
            }
            catch (GoogleApiException e)
            {
                if (e.Error.Code != 409)
                {
                    throw;
                }
            }
        }

        private PlaylistItem ToPlaylistItem(string playlistId, string videoId)
        {
            return new PlaylistItem
            {
                Id = playlistId,
                Snippet = new PlaylistItemSnippet
                {
                    PlaylistId = playlistId,
                    ResourceId = new ResourceId
                    {
                        VideoId = videoId,
                        Kind = _youtubeConstsProvider.VideoResourceKind,
                        PlaylistId = playlistId
                    }
                },
                Kind = _youtubeConstsProvider.YoutubePlaylistItemKind
            };
        }
    }
}
