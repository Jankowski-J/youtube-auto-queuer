var google = require('googleapis');
var googleAuth = require('./../google-auth');
var youtubeServiceProvider = require('./../youtube-service-provider');

var playlistsService = {};

function toPlaylistModel(basePlaylist) {
    return {
        playlistId: basePlaylist.id,
        name: basePlaylist.snippet.title,
        scheduledTime: null
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
                    data.unshift({
                        playlistId: "WL",
                        name: "Watch later",
                        scheduledTime: null
                    });
                    resolve(data);
                }
            });
    });
};

playlistsService.addVideosToPlaylist = function(videoIds, playlistId) {
    "use strict";
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
                function(error, data) {
                    if (error) {
                        if (error.code !== 409) {
                            console.log('error while adding videos to playlist:', error);
                        } else {
                            console.log('Video already in playlist');
                        }
                    }
                });
        }
        catch (error) {
            console.log(error);
        }
    }
}

module.exports = playlistsService;