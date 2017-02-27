using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Providers.Abstract;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    internal class YoutubeVideosService : IYoutubeVideosService
    {
        private readonly IYoutubeServiceProvider _serviceProvider;

        public YoutubeVideosService(IYoutubeServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<YoutubeVideoModel> GetLatestVideosFromChannel(UserCredential credential, string channelId, DateTime startDate)
        {
            const string parts = "id,snippet";
            var service = _serviceProvider.GetYoutubeService(credential);

            var searchQuery = service.Search.List(parts);
            searchQuery.ChannelId = channelId;
            searchQuery.MaxResults = 5;
            searchQuery.Order = SearchResource.ListRequest.OrderEnum.Date;
            var result = searchQuery.Execute();
      
            return result.Items.Select(x => new YoutubeVideoModel
            {
                Id = x.Id.VideoId,
                Name = x.Snippet.Title,
                PublishedAt = x.Snippet.PublishedAt
            });
        }
    }
}
