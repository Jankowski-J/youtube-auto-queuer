var google = require('googleapis');
var googleAuth = require('./../googleAuth');
var youtubeServiceProvider = require('./../youtube-service-provider');

var playlistsService = {};

function toPlaylistModel(basePlaylist) {
    return {
        playlistId: basePlaylist.id,
        name: basePlaylist.snippet.title
    };
}

playlistsService.getPlaylists = function() {
    var youtube = youtubeServiceProvider.getYoutubeService();

    var requestParams = {
        part: "id,snippet",
        mine: true,
        maxResults: 50,
    };

    return new Promise((resolve, reject) => {
        youtube.playlists.list(requestParams,
            (error, playlists, response) => {
                if (error) {
                    console.error('Error while getting playlists: ' + error);
                    reject(error);
                }
                if (playlists) {
                    data = playlists.items.map(toPlaylistModel);
                    resolve(data);
                }                
            });
    });
};

module.exports = playlistsService;