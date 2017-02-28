using Autofac;
using YoutubeQueuer.Lib.Providers;
using YoutubeQueuer.Lib.Services;

namespace YoutubeQueuer.Lib.Infrastructure
{
    public class QueuerLibAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<YoutubeServiceProvider>().AsImplementedInterfaces();
            RegisterServices(builder);
            base.Load(builder);
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<GoogleAuthService>().AsImplementedInterfaces();
            builder.RegisterType<YoutubeSubscriptionsService>().AsImplementedInterfaces();
            builder.RegisterType<YoutubePlaylistsService>().AsImplementedInterfaces();
            builder.RegisterType<YoutubeVideosService>().AsImplementedInterfaces();
        }
    }
}
