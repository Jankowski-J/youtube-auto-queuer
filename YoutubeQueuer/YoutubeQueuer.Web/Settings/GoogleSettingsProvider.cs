using System.Configuration;
using System.IO;
using System.Web;

namespace YoutubeQueuer.Web.Settings
{
    public class GoogleSettingsProvider
    {
        private static readonly string SecretFileName = ConfigurationManager.AppSettings["SecretsFileName"];

        public Stream GetSecretsStream()
        {
            var path = HttpContext.Current.Server.MapPath("~/" + SecretFileName);
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }
}