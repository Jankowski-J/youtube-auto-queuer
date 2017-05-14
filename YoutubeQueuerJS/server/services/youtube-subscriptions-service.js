var google = require('googleapis');
var googleAuth = require('./../google-auth');
var youtubeServiceProvider = require('./../youtube-service-provider');
var path = require('path');
var fs = require('fs');
var mkdirp = require('mkdirp');

var subscriptionsService = {};

function toSubscriptionModel(baseSub, settings) {
    var channelId = baseSub.snippet.resourceId.channelId;

    var matchingSetting = settings.find(s => {
        return s.channelId === channelId && s.isIncluded === true;
    });
    return {
        channelId: channelId,
        name: baseSub.snippet.title,
        isIncluded: !!matchingSetting
    };
}

function getSubscriptionsCore(subscriptionsSettings, nextPageToken) {
    var youtube = youtubeServiceProvider.getYoutubeService();
    var requestParams = {
        part: 'id,snippet',
        mine: true,
        maxResults: 50,
        pageToken: (!!nextPageToken) ? nextPageToken : null
    };

    subscriptionsSettings = subscriptionsSettings || [];

    var subscriptionsPromise = new Promise((resolve, reject) => {
        youtube.subscriptions.list(requestParams,
            (error, subscriptions, response) => {
                if (error) {
                    console.error('Error while getting subscriptions: ' + error);
                    reject(error);
                }

                if (subscriptions) {
                    var data = subscriptions.items
                        .map(s => toSubscriptionModel(s, subscriptionsSettings));
                    var result = {
                        data,
                        token: subscriptions.nextPageToken
                    };
                    resolve(result);
                }
            });
    });

    return subscriptionsPromise;
}

var getSettingsFilePath = function() {
    return path.join(process.env.APPDATA, '/YoutubeQueuer/subscriptions_settings.json')
};

var saveSubscriptionsSettings = function(settings) {
    var filtered = settings.subscriptions.filter(s => s.isIncluded);
    var serialized = JSON.stringify(filtered);
    var filePath = getSettingsFilePath();

    var pathName = path.dirname(filePath);
    fs.exists(pathName, exists => {
        if (!exists) {
             mkdirp.sync(pathName);
        }
               
        fs.writeFile(filePath, serialized, error => {
            if (error) {
                console.log('An error has occured while saving subscriptions settings:', error);
            }
        })
    });
};

var loadSubscriptionsSettings = function() {
    var filePath = getSettingsFilePath();

    var promise = new Promise((resolve, reject) => {
        fs.readFile(filePath, (error, data) => {
            if (error) {
                reject(error);
            }
            if (data) {
                var serialized = data.toString();
                var parsed = JSON.parse(serialized);
                resolve(parsed);
            }
        })
    });
    return promise;
}

function getAllSubscriptions() {
    var allData = [];

    var loadSubscriptionsWithSettings = function(settings) {
        var promise = new Promise((resolve, reject) => {
            var subsCallback = (subsResult) => {
                allData.push.apply(allData, subsResult.data);
                if (subsResult.token) {
                    getSubscriptionsCore(settings, subsResult.token)
                        .then(r => subsCallback(r))
                        .catch(error => reject(error));
                } else {
                    return resolve(allData);
                }
            };
            getSubscriptionsCore(settings)
                .then(result => subsCallback(result))
                .catch(error => console.log(error));
        });

        return promise;
    }

    var subscriptionsPromise = loadSubscriptionsSettings()
        .then(loadSubscriptionsWithSettings);

    return subscriptionsPromise;
}

subscriptionsService.getSubscriptions = function() {
    return getAllSubscriptions();
}

subscriptionsService.loadSubscriptionsSettings = loadSubscriptionsSettings;
subscriptionsService.saveSubscriptionsSettings = saveSubscriptionsSettings;

module.exports = subscriptionsService;