using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Owin;
using Owin;
using YoutubeQueuer.Lib.Infrastructure;
using YoutubeQueuer.Lib.Services.Abstract;
using YoutubeQueuer.Web.Controllers;
using YoutubeQueuer.Web.Filters;

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

            builder.RegisterFilterProvider();
            builder.RegisterModule<QueuerLibAutofacModule>();
            builder.RegisterType<AuthorizeYoutubeFilter>().AsActionFilterFor<PlaylistsController>()
                 .InstancePerRequest();
            builder.RegisterType<AuthorizeYoutubeFilter>().AsActionFilterFor<SubscriptionsController>();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
