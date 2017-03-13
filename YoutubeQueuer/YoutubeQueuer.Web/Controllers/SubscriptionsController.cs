using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        private readonly IUserSubscriptionsSettingsService _userSubscriptionsSettingsService;

        public SubscriptionsController(IYoutubeSubscriptionsService youtubeSubscriptionsService,
            IYoutubeVideosService youtubeVideosService,
            IUserSubscriptionsSettingsService userSubscriptionsSettingsService)
        {
            _youtubeSubscriptionsService = youtubeSubscriptionsService;
            _youtubeVideosService = youtubeVideosService;
            _userSubscriptionsSettingsService = userSubscriptionsSettingsService;
        }

        [AuthorizeYoutube]
        public ActionResult Index()
        {
            var subscriptions = _youtubeSubscriptionsService.GetUserSubscriptions(this.GetSessionCredential()).ToList();

            var mapped = subscriptions.Select(ToSubscriptionWebModel).ToList();
            var subscriptionsSettings =
                _userSubscriptionsSettingsService.GetUserSubscriptionsSettings(this.GetSessionCredential().UserId);

            if (!subscriptionsSettings.IsSuccess || !subscriptionsSettings.Data.Any())
            {
                mapped.ForEach(x => x.IsIncluded = true);
            }
            else
            {
                mapped = mapped
                    .Join(subscriptionsSettings.Data,
                        x => x.ChannelId,
                        y => y.ChannelId,
                        (x, y) =>
                        {
                            x.IsIncluded = y.IsIncluded;
                            return x;
                        }).ToList();
            }

            return View(mapped);
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

        [HttpPost]
        public ActionResult Index(IEnumerable<YoutubeSubscriptionWebModel> subscriptions)
        {
            var mapped = subscriptions.Select(x => new UserSubscriptionSettingsModel
            {
                IsIncluded = x.IsIncluded,
                ChannelId = x.ChannelId,
                UserName = this.GetSessionCredential().UserId
            }).ToList();
            var result = _userSubscriptionsSettingsService.SaveUserSubscriptionSettings(mapped,
                this.GetSessionCredential().UserId);

            if (result.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            throw new HttpException();
        }
    }
}