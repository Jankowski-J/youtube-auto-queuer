var google = require('googleapis');
var googleAuth = require('./googleAuth')

var playlistsService = {};

var youtube = undefined;

var initializeYoutube = function () {
    var yt = google.youtube({
        version: 'v3',
        auth: googleAuth.oauthClient
    });

    return yt;
};

playlistsService.getPlaylists = function (callback) {
    youtube = youtube || initializeYoutube();

    return youtube.playlists.list({
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
};


module.exports = playlistsService;