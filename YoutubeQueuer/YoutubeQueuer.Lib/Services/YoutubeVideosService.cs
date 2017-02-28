using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using YoutubeQueuer.Lib.Models;
using YoutubeQueuer.Lib.Providers.Abstract;
using YoutubeQueuer.Lib.Services.Abstract;

namespace YoutubeQueuer.Lib.Services
{
    internal class YoutubeVideosService : IYoutubeVideosService
    {
        private readonly IYoutubeServiceProvider _serviceProvider;
        private readonly IYoutubeConstsProvider _youtubeConstsProvider;

        public YoutubeVideosService(IYoutubeServiceProvider serviceProvider,
            IYoutubeConstsProvider youtubeConstsProvider)
        {
            _serviceProvider = serviceProvider;
            _youtubeConstsProvider = youtubeConstsProvider;
        }

        public IEnumerable<YoutubeVideoModel> GetLatestVideosFromChannel(string channelId, DateTime startDate, UserCredential credential)
        {
            var service = _serviceProvider.GetYoutubeService(credential);

            var searchQuery = service.Search.List(_youtubeConstsProvider.VideosListParts);
            searchQuery.ChannelId = channelId;
            searchQuery.MaxResults = 5;
            searchQuery.Order = SearchResource.ListRequest.OrderEnum.Date;
            var result = searchQuery.Execute();

            return result.Items
                .Where(x => x.Snippet.PublishedAt >= startDate).Select(x => new YoutubeVideoModel
                {
                    Id = x.Id.VideoId,
                    Name = x.Snippet.Title,
                    PublishedAt = x.Snippet.PublishedAt
                });
        }
    }
}
