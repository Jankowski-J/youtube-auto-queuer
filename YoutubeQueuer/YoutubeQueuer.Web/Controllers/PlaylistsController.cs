using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YoutubeQueuer.Common;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Services.Abstract;
using YoutubeQueuer.Web.Extensions;
using YoutubeQueuer.Web.Filters;
using YoutubeQueuer.Web.Models;

namespace YoutubeQueuer.Web.Controllers
{
    [AuthorizeYoutube]
    public class PlaylistsController : Controller
    {
        private readonly IYoutubePlaylistsService _playlistsService;
        private readonly IYoutubeVideosService _videosService;
        private readonly IYoutubeSubscriptionsService _youtubeSubscriptionsService;
        private readonly IUserSubscriptionsSettingsService _userSubscriptionsSettingsService;

        public PlaylistsController(IYoutubePlaylistsService playlistsService,
            IYoutubeVideosService videosService,
            IYoutubeSubscriptionsService youtubeSubscriptionsService,
            IUserSubscriptionsSettingsService userSubscriptionsSettingsService)
        {
            _playlistsService = playlistsService;
            _videosService = videosService;
            _youtubeSubscriptionsService = youtubeSubscriptionsService;
            _userSubscriptionsSettingsService = userSubscriptionsSettingsService;
        }
        
        public ActionResult Index()
        {
            var playlists = _playlistsService.GetUserPlaylists(this.GetSessionCredential());

            var mapped = playlists.Select(ToPlaylistWebModel).ToList();
            return View(mapped);
        }

        private static YoutubePlaylistWebModel ToPlaylistWebModel(YoutubePlaylistModel playlist)
        {
            return new YoutubePlaylistWebModel
            {
                Url = $"https://www.youtube.com/playlist?list={playlist.Id}",
                Name = playlist.Name,
                Id = playlist.Id
            };
        }

        [HttpPost]
        public ActionResult AddVideos(string playlistId)
        {
            var subscriptions = _youtubeSubscriptionsService.GetUserSubscriptions(this.GetSessionCredential()).ToList();
            var settings =
                _userSubscriptionsSettingsService.GetUserSubscriptionsSettings(this.GetAuthenticatedUserName());

            var validSubscriptions = !settings.Data.Any()
                ? subscriptions
                : subscriptions.Where(x => IsSubscriptionIncluded(settings, x)).ToList();

            var videos = GetNewestVideosFromSubscriptions(validSubscriptions);

            var result = _playlistsService.AddVideosToPlaylist(videos.Select(x => x.Id).ToList(), playlistId,
                this.GetSessionCredential());

            if (!result.IsSuccess)
            {
                throw new HttpException();
            }

            return RedirectToAction("Index");
        }

        private static bool IsSubscriptionIncluded(Result<IEnumerable<UserSubscriptionSettingsModel>> settings,
            YoutubeSubscriptionModel subscription)
        {
            var setting = settings.Data.FirstOrDefault(y => y.ChannelId == subscription.ChannelId);
            return setting != null && setting.IsIncluded;
        }

        private IEnumerable<YoutubeVideoModel> GetNewestVideosFromSubscriptions(IEnumerable<YoutubeSubscriptionModel> subscriptions)
        {
            return subscriptions.Select(x => _videosService.GetLatestVideosFromChannel(x.ChannelId,
                        DateTime.Now.Date, this.GetSessionCredential()).FirstOrDefault())
                .Where(x => x != null)
                .ToList();
        }
    }
}