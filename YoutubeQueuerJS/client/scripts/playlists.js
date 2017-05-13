var service = new Services.PlaylistsService();
function feedVideos(playlist) {
    var model = { playlistId: playlist.playlistId() };

    var data = JSON.stringify(model);

    var headers = new Headers();
    headers.append("Content-Type", "application/json");

    fetch('/api/feedvideos', {
        method: 'POST',
        body: data,
        headers
    }).catch(error => {
        console.log('Error while feeding videos:', error);
    });
}

function addScheduledTime(obj) {
    obj.scheduledTime(0);
}

function saveScheduledTime(scheduleData) {
    var headers = new Headers();
    headers.append("Content-Type", "application/json");
    var model = {
        playlistId: scheduleData.playlistId(),
        scheduledTime: scheduleData.scheduledTime()
    };
    var data = JSON.stringify(model);
    fetch('/api/subscriptions/schedule', {
        method: 'POST',
        body: data,
        headers
    }).catch(error => {
        console.log('Error while scheduling feed time:', error);
    });
}

var hours = [...Array(24).keys()].map(n => n);

var subs = service.getPlaylists(playlists => {
    var observablePlaylists = playlists.map(p => {
        return {
            playlistId: ko.observable(p.playlistId),
            name: p.name,
            scheduledTime: ko.observable(p.scheduledTime)
        }
    });
    ko.applyBindings({
        playlists: ko.observableArray(observablePlaylists),
        feedVideos,
        addScheduledTime,
        hours
    });
});