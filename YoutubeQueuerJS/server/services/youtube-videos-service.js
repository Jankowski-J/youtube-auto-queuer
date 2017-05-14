var google = require('googleapis');
var googleAuth = require('./../google-auth');
var youtubeServiceProvider = require('./../youtube-service-provider');

var videosService = {};

function toVideoModel(baseVideo) {
    return {
        'id': baseVideo.id.videoId,
        'name': baseVideo.snippet.title,
        'publishedAt': new Date(Date.parse(baseVideo.snippet.publishedAt))
    };
}

videosService.getLatestVideosFromChannel = function(channelId, maxDate) {
    var youtube = youtubeServiceProvider.getYoutubeService();

    var requestParams = {
        part: 'id,snippet',
        maxResults: 50,
        order: 'date',
        channelId: channelId
    };

    maxDate = maxDate || new Date();
    maxDate.setHours(0, 0, 0, 0);

    var videosPromise = new Promise((resolve, reject) => {
        youtube.search.list(requestParams,
            (error, videos, response) => {
                if (error) {
                    console.error('Error while getting videos: ' + error);
                    reject(error);
                }

                if (videos) {
                    var data = videos.items.map(toVideoModel);
                    var filtered = data.filter(x => x.publishedAt.getTime() >= maxDate.getTime());
                    resolve(filtered);
                }
            });
    });

    return videosPromise;
}

module.exports = videosService;