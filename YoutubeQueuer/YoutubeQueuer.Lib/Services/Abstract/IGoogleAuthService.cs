using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using YoutubeQueuer.Lib.Models;

namespace YoutubeQueuer.Lib.Services.Abstract
{
    public interface IGoogleAuthService
    {
        Task<IEnumerable<YoutubeSubscriptionModel>> GetUserSubscriptions(UserCredential credential);
        Task<UserCredential> AuthorizeUser(Stream stream);
    }
}
