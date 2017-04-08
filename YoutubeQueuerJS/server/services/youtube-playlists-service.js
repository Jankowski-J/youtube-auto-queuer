var google = require('googleapis');
var googleAuth = require('./../googleAuth');
var youtubeServiceProvider = require('./../youtube-service-provider');

var playlistsService = {};

playlistsService.getPlaylists = function(callback) {
    var youtube = youtubeServiceProvider.getYoutubeService();

    return youtube.playlists.list({
        part: "id,snippet",
        mine: true
    }, (err, playlists, response) => {
        var data;
        if (err) {
            console.error('Error while getting playlists: ' + err);
            playlists = [];
        }
        if (playlists) {
            data = playlists.items.map(e => {
                return {
                    playlistId: e.id,
                    name: e.snippet.title
                }
            });
        }
        callback(data, response, err);
    });
};

module.exports = playlistsService;