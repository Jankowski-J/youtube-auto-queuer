using System.Linq;
using System.Web.Mvc;
using YoutubeQueuer.Lib.Services.Abstract;
using YoutubeQueuer.Web.Extensions;
using YoutubeQueuer.Web.Filters;
using YoutubeQueuer.Web.Models;

namespace YoutubeQueuer.Web.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly IYoutubePlaylistsService _playlistsService;

        public PlaylistsController(IYoutubePlaylistsService playlistsService)
        {
            _playlistsService = playlistsService;
        }

        [AuthorizeYoutube]
        public ActionResult Index()
        {
            var playlists = _playlistsService.GetUserPlaylists(this.GetSessionCredential());

            var mapped = playlists.Select(x => new PlaylistViewModel
            {
                Url = $"https://www.youtube.com/playlist?list={x.Id}",
                Name = x.Name
            }).ToList();
            return View(mapped);
        }
    }
}