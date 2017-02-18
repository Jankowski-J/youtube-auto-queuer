using Autofac;
using YoutubeQueuer.Lib.Services;

namespace YoutubeQueuer.Lib.Infrastructure
{
    public class QueuerLibAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GoogleAuthService>().AsImplementedInterfaces();
            builder.RegisterType<YoutubeSubscriptionsService>().AsImplementedInterfaces();
            base.Load(builder);
        }
    }
}
