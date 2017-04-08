var google = require('googleapis');
var googleAuth = require('./../googleAuth');
var youtubeServiceProvider = require('./../youtube-service-provider');

var subscriptionsService = {};

function getSubscriptionsCore(callback, nextPageToken) {
    var youtube = youtubeServiceProvider.getYoutubeService();
    var requestParams = {
        part: "id,snippet",
        mine: true,
        maxResults: 50,
        pageToken: (!!nextPageToken) ? nextPageToken : null
    };

    return youtube.subscriptions.list(requestParams,
        (err, subscriptions, response) => {
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
            callback(data, subscriptions.nextPageToken);
        });
}

function getAllSubscriptions(callback) {
    var allData = [];

    var subsCallback = (subs, token) => {
        allData.push.apply(allData, subs);
        if(token) {
            getSubscriptionsCore(subsCallback, token);
        } else {
            callback(allData);
        }
    };

    var subscriptions = getSubscriptionsCore(subsCallback);
}

subscriptionsService.getSubscriptions = function(callback) {
    return getAllSubscriptions(callback);
}

module.exports = subscriptionsService;