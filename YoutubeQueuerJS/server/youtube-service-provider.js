var google = require('googleapis');
var googleAuth = require('./google-auth')

var youtube = undefined;

var initializeYoutubeService = function() {
    var yt = google.youtube({
        version: 'v3',
        auth: googleAuth.oauthClient
    });

    //var people = google.people("v1");

    return yt;
};

var youtubeServiceProvider = {};
youtubeServiceProvider.getYoutubeService = function() {
    if (!youtube) {
        youtube = initializeYoutubeService();
    }

    return youtube;
}

module.exports = youtubeServiceProvider;
