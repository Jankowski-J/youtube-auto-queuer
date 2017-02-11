using System.Configuration;
using System.IO;
using System.Web;

namespace YoutubeQueuer.Web.Settings
{
    public class GoogleSettingsProvider
    {
        private static readonly string SettingsFileName = ConfigurationManager.AppSettings["GoogleAccountFileName"];

        public Stream GetSerializedSettings()
        {
            var path = HttpContext.Current.Server.MapPath(SettingsFileName);
            var settingsStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return settingsStream;;
        }
    }
}