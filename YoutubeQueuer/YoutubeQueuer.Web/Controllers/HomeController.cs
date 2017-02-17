using System.Threading.Tasks;
using System.Web.Mvc;
using YoutubeQueuer.Lib.Services.Abstract;
using YoutubeQueuer.Web.Extensions;
using YoutubeQueuer.Web.Settings;

namespace YoutubeQueuer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGoogleAuthService _googleAuthService;

        public HomeController(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;
        }

        public ActionResult Index()
        {
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

        public async Task<ActionResult> Authorize()
        {
            var stream = new GoogleSettingsProvider().GetSecretsStream();
            var credential = await _googleAuthService.AuthorizeUser(stream);

            this.SetSessionCredential(credential);

            return View("Index");
        }
    }
}