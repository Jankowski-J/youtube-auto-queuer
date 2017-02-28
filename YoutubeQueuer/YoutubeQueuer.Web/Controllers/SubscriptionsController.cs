using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Services.Abstract;
using YoutubeQueuer.Web.Extensions;
using YoutubeQueuer.Web.Filters;
using YoutubeQueuer.Web.Models;

namespace YoutubeQueuer.Web.Controllers
{
    public class SubscriptionsController : Controller
    {
        private readonly IYoutubeSubscriptionsService _youtubeSubscriptionsService;
        private readonly IYoutubeVideosService _youtubeVideosService;

        public SubscriptionsController(IYoutubeSubscriptionsService youtubeSubscriptionsService,
            IYoutubeVideosService youtubeVideosService)
        {
            _youtubeSubscriptionsService = youtubeSubscriptionsService;
            _youtubeVideosService = youtubeVideosService;
        }

        [AuthorizeYoutube]
        public async Task<ActionResult> Index()
        {
            var subscriptions = (await _youtubeSubscriptionsService.GetUserSubscriptions(this.GetSessionCredential())).ToList();

            return View(subscriptions.Select(ToSubscriptionWebModel));
        }

        private YoutubeSubscriptionWebModel ToSubscriptionWebModel(YoutubeSubscriptionModel model)
        {
            var latestVideo =
                _youtubeVideosService.GetLatestVideosFromChannel(this.GetSessionCredential(), model.ChannelId, DateTime.Now)
                    .First();
            return new YoutubeSubscriptionWebModel
            {
                ChannelId = model.ChannelId,
                ChannelName = model.ChannelName,
                LatestVideo = new YoutubeVideoWebModel
                {
                    Id = latestVideo.Id,
                    Name = latestVideo.Name,
                    PublishedAt = latestVideo.PublishedAt
                }
            };
        }
    }
}