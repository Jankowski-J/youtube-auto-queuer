using System.IO;
using System.Threading.Tasks;
using YoutubeQueuer.Lib.Models;

namespace YoutubeQueuer.Lib.Services.Abstract
{
    public interface IGoogleAuthService
    {
        Task<string> GetAuthorizationTokenForApp(Stream serializedSettings, GoogleAuthorizationScope scope);
    }
}
