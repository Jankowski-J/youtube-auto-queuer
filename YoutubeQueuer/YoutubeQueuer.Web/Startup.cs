using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Owin;
using Owin;
using YoutubeQueuer.Lib.Infrastructure;

[assembly: OwinStartupAttribute(typeof(YoutubeQueuer.Web.Startup))]
namespace YoutubeQueuer.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            SetupDependencyResolution();
        }

        private static void SetupDependencyResolution()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<QueuerLibAutofacModule>();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
