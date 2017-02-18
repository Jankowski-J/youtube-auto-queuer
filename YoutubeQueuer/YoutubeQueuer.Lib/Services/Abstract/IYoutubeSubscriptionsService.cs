using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using YoutubeQueuer.Lib.Models;

namespace YoutubeQueuer.Lib.Services.Abstract
{
    public interface IYoutubeSubscriptionsService
    {
        Task<IEnumerable<YoutubeSubscriptionModel>> GetUserSubscriptions(UserCredential credential);
    }
}
