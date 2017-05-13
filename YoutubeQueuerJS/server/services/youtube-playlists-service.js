var google = require('googleapis');
var googleAuth = require('./../google-auth');
var youtubeServiceProvider = require('./../youtube-service-provider');
var feedSchedulingSerivce = require('./feed-scheduling-service');

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
        part: 'id,snippet',
        mine: true,
        maxResults: 50,
    };

    return new Promise((resolve, reject) => {
        var playlists = [];
        youtube.playlists.list(requestParams,
            (error, data, response) => {
                if (error) {
                    console.error('Error while getting playlists:' + error);
                    reject(error);
                }
                if (data) {
                    var mapped = data.items.map(toPlaylistModel);
                    mapped.unshift({
                        playlistId: "WL",
                        name: "Watch later"
                    });
                    playlists = mapped;
                    feedSchedulingSerivce.getScheduledTimesForPlaylists()
                        .then(schedulingSettings => {
                            for (let playlist of playlists) {
                                var setting = schedulingSettings.find(s => s.playlistId === playlist.playlistId);
                                if (setting) {
                                    playlist.scheduledTime = setting.scheduledTime;
                                }
                            }
                            resolve(playlists);
                        }).catch(() => resolve(playlists));
                }
            });
    });
};

playlistsService.addVideosToPlaylist = function(videoIds, playlistId) {
    'use strict';
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