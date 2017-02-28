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

        public YoutubeVideosService(IYoutubeServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<YoutubeVideoModel> GetLatestVideosFromChannel(string channelId, DateTime startDate, UserCredential credential)
        {
            const string parts = "id,snippet";
            var service = _serviceProvider.GetYoutubeService(credential);

            var searchQuery = service.Search.List(parts);
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
