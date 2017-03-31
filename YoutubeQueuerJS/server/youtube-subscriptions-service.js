var google = require('googleapis');
var googleAuth = require('./googleAuth');
var youtubeServiceProvider = require('./youtube-service-provider');

var subscriptionsService = {};

subscriptionsService.getSubscriptions = function(callback) {
    var youtube = youtubeServiceProvider.getYoutubeService();

    return youtube.subscriptions.list({
        part: "id,snippet",
        mine: true
    }, (err, data, response) => {
        var mappedData;
        if (err) {
            console.error('Error: ' + err);
        }
        if (data) {
            mappedData = data.items.map(e => {
                return {
                    playlistId: e.id,
                    name: e.snippet.title
                }
            });
        }
        if (response) {
            console.log('Status code: ' + response.statusCode);
        }
        callback(mappedData, response, err);
    });
}

module.exports = subscriptionsService;