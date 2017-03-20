using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using YoutubeQueuer.Lib.Services.Abstract;
using YoutubeQueuer.Web.Settings;

namespace YoutubeQueuer.Web.Filters
{
    public class AuthorizeYoutubeFilter : IActionFilter
    {
        private readonly IGoogleAuthService _googleAuthService;

        public AuthorizeYoutubeFilter(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var attribute = GetAttributeFromController(filterContext) ??
                            GetAttributeFromMethod(filterContext);

            if (attribute == null)
            {
                return;
            }

            var secrets = new GoogleSettingsProvider().GetSecretsStream();
            var task = _googleAuthService.AuthorizeUser(secrets);

            //if (!task.IsCompleted)
            //{
            //    task.RunSynchronously();
            //}
            filterContext.HttpContext.Session["Credentials"] = task.Result;
        }

        private static object GetAttributeFromMethod(ActionExecutingContext filterContext)
        {
            return filterContext
                .ActionDescriptor.GetCustomAttributes(typeof(AuthorizeYoutubeAttribute), true)
                .SingleOrDefault();
        }

        private static object GetAttributeFromController(ActionExecutingContext filterContext)
        {
            var controllerAttributes = filterContext.ActionDescriptor
                .ControllerDescriptor
                .GetCustomAttributes(typeof(AuthorizeYoutubeAttribute), true);

            return controllerAttributes.SingleOrDefault();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }
    }
}