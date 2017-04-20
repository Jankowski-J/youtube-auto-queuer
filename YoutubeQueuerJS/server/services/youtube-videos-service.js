var google = require('googleapis');
var googleAuth = require('./../google-auth');
var youtubeServiceProvider = require('./../youtube-service-provider');

var videosService = {};

function toVideoModel(baseVideo) {
    return {
        'id': baseVideo.id.videoId,
        'name': baseVideo.snippet.title,
        'publishedAt': baseVideo.snippet.publishedAt
    };
}

videosService.getLatestVideosFromChannel = function getLatestVideosFromChannel(channelId) {
    var youtube = youtubeServiceProvider.getYoutubeService();

    var requestParams = {
        part: "id,snippet",
        mine: true,
        maxResults: 50,
        order: 'date'
    };

    return new Promise((resolve, reject) => {
        youtube.search.list(requestParams,
            (error, videos, response) => {
                if (error) {
                    console.error('Error while getting videos: ' + error);
                    reject(error);
                }

                if (videos) {
                    var data = videos.items.map(toVideoModel);
                    resolve([data[0]]);
                }
            });
    });
}

module.exports = videosService;