using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;

namespace YoutubeQueuer.Lib.Services.Abstract
{
    public interface IGoogleAuthService
    {
        Task<UserCredential> AuthorizeUser(Stream stream);
    }
}
