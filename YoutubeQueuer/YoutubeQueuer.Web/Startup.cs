using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(YoutubeQueuer.Web.Startup))]
namespace YoutubeQueuer.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
