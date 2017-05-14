var playlistsService = require('./youtube-playlists-service');
var subscriptionsService = require('./youtube-subscriptions-service');
var videosService = require('./youtube-videos-service');

var youtubeVideosFeedingService = {};

function toVideoFeedingPromise(sub, playlistId) {
    return function() {
        return videosService.getLatestVideosFromChannel(sub.channelId)
            .then(videos => {
                var ids = videos.map(v => v.id);
                playlistsService.addVideosToPlaylist(ids, playlistId);
            })
            .catch(error =>
                console.log('An error occured while getting videos from channel: ' + error));
    }
}

function feedVideosToPlaylist(playlistId) {
    subscriptionsService.getSubscriptions()
        .then(subs => {
            var validSubs = subs.filter(s => s.isIncluded);

            var promises = validSubs.map(s => toVideoFeedingPromise(s, playlistId));

            promises.reduce((cur, next) => {
                return cur.then(next);
            }, Promise.resolve());
        });
}

youtubeVideosFeedingService.feedVideosToPlaylist = feedVideosToPlaylist;

module.exports = youtubeVideosFeedingService;