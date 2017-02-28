using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using YoutubeQueuer.Common;
using YoutubeQueuer.Lib.Models;

namespace YoutubeQueuer.Lib.Services.Abstract
{
    public interface IYoutubePlaylistsService
    {
        IEnumerable<YoutubePlaylistModel> GetUserPlaylists(UserCredential credential);
        Result AddVideosToPlaylist(IEnumerable<string> videoIds, string playlistId, UserCredential credential);
    }
}
