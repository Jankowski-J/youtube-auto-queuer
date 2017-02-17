using System.Threading.Tasks;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
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
            var authService = new GoogleAuthService();

            await authService.GetUserSubscriptions(GetSessionCredential());
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> Authorize()
        {
            var authService = new GoogleAuthService();
            var stream = new GoogleSettingsProvider().GetSecretsStream();
            var credential = await authService.AuthorizeUser(stream);

            SetSessionCredential(credential);

            return View("Index");
        }

        private void SetSessionCredential(UserCredential credential)
        {
            Session["Credentials"] = credential;
        }

        public UserCredential GetSessionCredential()
        {
            return Session["Credentials"] as UserCredential;
        }
    }
}