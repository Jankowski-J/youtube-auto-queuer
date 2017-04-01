var google = require('googleapis');
var googleAuth = require('./googleAuth');
var youtubeServiceProvider = require('./youtube-service-provider');

var subscriptionsService = {};

subscriptionsService.getSubscriptions = function(callback) {
    var youtube = youtubeServiceProvider.getYoutubeService();

    return youtube.subscriptions.list({
        part: "id,snippet",
        mine: true,
        maxResults: 50
    }, (err, subscriptions, response) => {
        var data;
        if (err) {
            console.error('Error while getting subscriptions: ' + err);
            data = [];
        }
        if (subscriptions) {
            data = subscriptions.items.map(e => {
                return {
                    channelId: e.snippet.resourceId.channelId,
                    name: e.snippet.title
                }
            });
        }
        callback(data, response, err);
    });
}

module.exports = subscriptionsService;