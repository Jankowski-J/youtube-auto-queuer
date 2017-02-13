using System.Threading.Tasks;
using System.Web.Mvc;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Services;
using YoutubeQueuer.Web.Settings;

namespace YoutubeQueuer.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> About()
        {
            var settingsProvider = new GoogleSettingsProvider();
            var authService = new GoogleAuthService();
            await authService.GetAuthorizationTokenForApp(settingsProvider.GetSerializedSettings(),
                GoogleAuthorizationScope.Youtube);

            await authService.GetUserSubscriptions(authService.GetAuthorizedUserId());
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> Authorize(string userName)
        {
            var authService = new GoogleAuthService();
            var stream = new GoogleSettingsProvider().GetSecretsStream();
            await authService.AuthorizeUser(userName, "aa", stream);

            return View("Index");
        }
    }
}