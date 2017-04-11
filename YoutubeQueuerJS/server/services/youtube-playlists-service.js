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
                    var data = playlists.items.map(toPlaylistModel);
                    resolve(data);
                }
            });
    });
};

playlistsService.addVideosToPlaylist = function(videoIds, playlistId) {
    var youtube = youtubeServiceProvider.getYoutubeService();

    var videosToAdd = videoIds.map(v => {
        return {
            resource: {
                snippet: {
                    playlistId: playlistId,
                    resourceId: {
                        'videoId': v,
                        'kind': 'youtube#video',
                    }
                }
            },
            part: 'id,snippet',
            resourceId: {
                'videoId': v,
                'kind': 'youtube#video',
            }
        }
    });

    for (let videoPayload of videosToAdd) {
        try {
            youtube.playlistItems.insert(videoPayload, 
                function(err, data) {
                    // TODO: something useful here
            });
        }
        catch (error) {
            console.log(error);
        }
    }
}

module.exports = playlistsService;