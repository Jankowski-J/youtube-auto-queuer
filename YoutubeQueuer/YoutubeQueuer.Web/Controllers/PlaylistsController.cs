using System;
using System.Linq;
using System.Web;
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
        private readonly IYoutubeVideosService _videosService;
        private readonly IYoutubeSubscriptionsService _youtubeSubscriptionsService;

        public PlaylistsController(IYoutubePlaylistsService playlistsService,
            IYoutubeVideosService videosService,
            IYoutubeSubscriptionsService youtubeSubscriptionsService)
        {
            _playlistsService = playlistsService;
            _videosService = videosService;
            _youtubeSubscriptionsService = youtubeSubscriptionsService;
        }

        [AuthorizeYoutube]
        public ActionResult Index()
        {
            var playlists = _playlistsService.GetUserPlaylists(this.GetSessionCredential());

            var mapped = playlists.Select(x => new PlaylistWebModel
            {
                Url = $"https://www.youtube.com/playlist?list={x.Id}",
                Name = x.Name,
                Id = x.Id
            }).ToList();
            return View(mapped);
        }

        [HttpPost]
        public ActionResult AddVideos(string playlistId)
        {
            var subscriptions = _youtubeSubscriptionsService.GetUserSubscriptions(this.GetSessionCredential()).ToList();

            var videos =
                subscriptions.Select(
                        x => _videosService.GetLatestVideosFromChannel(x.ChannelId,
                            DateTime.Now.Date, this.GetSessionCredential()).FirstOrDefault())
                    .Where(x => x != null);

            var result = _playlistsService.AddVideosToPlaylist(videos.Select(x => x.Id).ToList(), playlistId,
                this.GetSessionCredential());

            if (!result.IsSuccess)
            {
                throw new HttpException();
            }

            return RedirectToAction("Index");
        }
    }
}