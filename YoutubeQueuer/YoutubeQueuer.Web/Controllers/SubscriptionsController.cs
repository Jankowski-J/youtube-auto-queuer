using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YoutubeQueuer.Lib.Services.Abstract;
using YoutubeQueuer.Web.Extensions;
using YoutubeQueuer.Web.Filters;
using YoutubeQueuer.Web.Models;

namespace YoutubeQueuer.Web.Controllers
{
    [AuthorizeYoutube]
    public class SubscriptionsController : Controller
    {
        private readonly IYoutubeSubscriptionsService _youtubeSubscriptionsService;

        public SubscriptionsController(IYoutubeSubscriptionsService youtubeSubscriptionsService)
        {
            _youtubeSubscriptionsService = youtubeSubscriptionsService;
        }

        // GET: Subscriptions
        public async Task<ActionResult> Index()
        {
            var subscriptions = await _youtubeSubscriptionsService.GetUserSubscriptions(this.GetSessionCredential());

            return View(subscriptions.Select(x => new SubscriptionModel
            {
                ChannelId = x.ChannelId,
                ChannelName = x.ChannelName
            }));
        }
    }
}