using System.Web.Http;
using System.Web.Mvc;

namespace YoutubeQueuer.Web.Controllers
{
    public class PlaylistsApiController : ApiController
    {
        public PlaylistsController _playlistsController { get; set; }

        [System.Web.Http.HttpGet]
        public ActionResult Index()
        {
            return _playlistsController.Index();
        }
    }
}