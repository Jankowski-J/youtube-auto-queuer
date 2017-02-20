using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using YoutubeQueuer.Lib.Models;

namespace YoutubeQueuer.Lib.Services.Abstract
{
    public interface IYoutubePlaylistsService
    {
        IEnumerable<YoutubePlaylistModel> GetUserPlaylists(UserCredential credential);
    }
}
