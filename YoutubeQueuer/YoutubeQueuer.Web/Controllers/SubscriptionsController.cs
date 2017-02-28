using System;
using System.Linq;
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
        public ActionResult Index()
        {
            var subscriptions = _youtubeSubscriptionsService.GetUserSubscriptions(this.GetSessionCredential()).ToList();

            return View(subscriptions.Select(ToSubscriptionWebModel));
        }

        private YoutubeSubscriptionWebModel ToSubscriptionWebModel(YoutubeSubscriptionModel model)
        {
            var latestVideo =
                _youtubeVideosService.GetLatestVideosFromChannel(model.ChannelId, DateTime.Now.Date, this.GetSessionCredential())
                    .FirstOrDefault();
            return new YoutubeSubscriptionWebModel
            {
                ChannelId = model.ChannelId,
                ChannelName = model.ChannelName,
                LatestVideo = latestVideo != null
                    ? new YoutubeVideoWebModel
                    {
                        Id = latestVideo.Id,
                        Name = latestVideo.Name,
                        PublishedAt = latestVideo.PublishedAt
                    }
                    : null
            };
        }
    }
}