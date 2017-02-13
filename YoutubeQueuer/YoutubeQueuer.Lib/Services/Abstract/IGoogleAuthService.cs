using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace YoutubeQueuer.Lib.Services.Abstract
{
    public interface IGoogleAuthService
    {
        Task<IEnumerable<string>> GetUserSubscriptions(string userName);
        Task AuthorizeUser(string userName, Stream stream);
        string GetAuthorizedUserId();
    }
}
