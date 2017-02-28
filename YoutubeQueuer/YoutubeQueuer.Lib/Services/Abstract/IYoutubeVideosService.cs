using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using YoutubeQueuer.Lib.Models;

namespace YoutubeQueuer.Lib.Services.Abstract
{
    public interface IYoutubeVideosService
    {
        IEnumerable<YoutubeVideoModel> GetLatestVideosFromChannel(UserCredential credential, string channelId, DateTime startDate);
    }
}
