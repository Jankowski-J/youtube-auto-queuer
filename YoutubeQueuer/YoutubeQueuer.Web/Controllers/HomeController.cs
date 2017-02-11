using System.Threading.Tasks;
using System.Web.Mvc;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Services;
using YoutubeQueuer.Web.Settings;

namespace YoutubeQueuer.Web.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var settingsProvider = new GoogleSettingsProvider();
            var authService = new GoogleAuthService();
            await authService.GetAuthorizationTokenForApp(settingsProvider.GetSerializedSettings(),
                GoogleAuthorizationScope.Youtube);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}