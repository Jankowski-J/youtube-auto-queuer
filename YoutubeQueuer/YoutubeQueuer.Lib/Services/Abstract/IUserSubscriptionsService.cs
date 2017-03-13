using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using YoutubeQueuer.Common;
using YoutubeQueuer.Lib.Models;

namespace YoutubeQueuer.Lib.Services.Abstract
{
    public interface IUserSubscriptionsService
    {
        Result<IEnumerable<UserSubscriptionModel>> GetOnlyIncludedSubscriptions(UserCredential credential);
    }
}
