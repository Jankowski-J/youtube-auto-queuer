using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;

namespace YoutubeQueuer.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static void SetSessionCredential(this Controller controller, UserCredential credential)
        {
            controller.Session["Credentials"] = credential;
        }

        public static UserCredential GetSessionCredential(this Controller controller)
        {
            return controller.Session["Credentials"] as UserCredential;
        }

        public static string GetAuthenticatedUserName(this Controller controller)
        {
            var credential = GetSessionCredential(controller);

            return credential?.UserId;
        }
    }
}