var google = require('googleapis');
var googleAuth = require('./../googleAuth');
var youtubeServiceProvider = require('./../youtube-service-provider');

var subscriptionsService = {};

function toSubscriptionModel(baseSub) {
    return {
        channelId: baseSub.snippet.resourceId.channelId,
        name: baseSub.snippet.title
    };
}

function getSubscriptionsCore(nextPageToken) {
    var youtube = youtubeServiceProvider.getYoutubeService();
    var requestParams = {
        part: "id,snippet",
        mine: true,
        maxResults: 50,
        pageToken: (!!nextPageToken) ? nextPageToken : null
    };

    return new Promise((resolve, reject) => {
        youtube.subscriptions.list(requestParams,
            (error, subscriptions, response) => {
                if (error) {
                    console.error('Error while getting subscriptions: ' + error);
                    reject(error);
                }

                if (subscriptions) {
                    var data = subscriptions.items.map(toSubscriptionModel);
                    var result = {
                        data,
                        token: subscriptions.nextPageToken
                    };
                    resolve(result);
                }
            });
    });
}

function getAllSubscriptions(callback) {
    var allData = [];

    var promise = new Promise((resolve, reject) => {
        var subsCallback = (subsResult) => {
            allData.push.apply(allData, subsResult.data);
            if (subsResult.token) {
                getSubscriptionsCore(subsResult.token)
                    .then(r => subsCallback(r))
                    .catch(error => reject(error));
            } else {
                return resolve(allData);
            }
        };
        getSubscriptionsCore()
            .then(result => subsCallback(result))
            .catch(error => console.log(error));
    });

    return promise;
}

subscriptionsService.getSubscriptions = function() {
    return getAllSubscriptions();
}

module.exports = subscriptionsService;