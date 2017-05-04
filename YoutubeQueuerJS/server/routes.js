var googleAuth = require('./google-auth');
var path = require('path');

var playlistsService = require('./services/youtube-playlists-service');
var subscriptionsService = require('./services/youtube-subscriptions-service');
var videosService = require('./services/youtube-videos-service');
var youtubeVideosFeedingService = require('./services/youtube-videos-feeding-service');

var routesConfig = {};

function setupAuthorizationUrls(app) {
    app.get('/authorize', (req, res) => {
        googleAuth.authorize(res, '/playlists');
    });

    app.get('/authorized', (req, res) => {
        googleAuth.handleAuthorizationResponse(req, res);
    });
}

function setupApi(app) {
    "use strict";
    app.get("/api/subscriptions", (req, res) => {
        subscriptionsService.getSubscriptions()
            .then(data => {
                res.send(JSON.stringify(data));
            });
    });

    app.get("/api/playlists", (req, res) => {
        playlistsService.getPlaylists()
            .then(data => {
                res.send(JSON.stringify(data));
            });
    });

    app.post("/api/feedvideos", (req, res) => {
        var playlistId = req.body.playlistId;

        youtubeVideosFeedingService.feedVideosToPlaylist(playlistId);
    })

    app.post("/api/subscriptions/", (req, res) => {
        subscriptionsService.saveSubscriptionsSettings(req.body);
        res.status(200).end();
    });
}

function setupViews(app) {
    app.get("/subscriptions", (req, res) => {
        res.sendFile(path.join(__dirname, "/../views/subscriptions.html"));
    });

    app.get("/playlists", (req, res) => {
        res.sendFile(path.join(__dirname + "/../views/playlists.html"));
    });
}

routesConfig.configure = function(app, port) {
    googleAuth.configure(port);

    app.use(googleAuth.middleware);
    setupAuthorizationUrls(app);
    setupApi(app);
    setupViews(app);

    app.get("/", (req, res) => {
        res.sendFile(path.join(__dirname, "/../index.html"));
    });
};

module.exports = routesConfig;