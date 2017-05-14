var Services = Services || {};

Services.PlaylistsService = function PlaylistsService() {
    var that = this;
    that.getPlaylists = function(callback) {
        callback = callback || function() { };
        return fetch("/api/playlists")
            .then(response => response.json())
            .then(data => callback(data));
    }
    return that;
}

