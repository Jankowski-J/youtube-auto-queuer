var googleAuth = require('./googleAuth');
var path = require('path');
var playlistsService = require('./services/youtube-playlists-service');
var subscriptionsService = require('./services/youtube-subscriptions-service');

var routesConfig = {};

function setupAuthorizationUrls(app) {
    app.get("/authorize", (req, res) => {
        res.redirect(googleAuth.authorizationUrl);
    });

    app.get("/authorized", (req, res) => {
        googleAuth.oauthClient.getToken(req.query.code, function(err, tokens) {
            if (err) {
                console.log(err);
                return;
            }
            googleAuth.oauthClient.setCredentials(tokens);
            res.redirect("/subscriptions");
        });
    });
}

function setupApi(app) {
    app.get("/api/subscriptions", (req, res) => {
        subscriptionsService.getSubscriptions(data => {
            res.send(JSON.stringify(data));
        });
    });

    app.get("/api/playlists", (req, res) => {
        playlistsService.getPlaylists(data => {
            res.send(JSON.stringify(data));
        });
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

    setupAuthorizationUrls(app);
    setupApi(app);
    setupViews(app);

    app.get("/", (req, res) => {
        res.sendFile(path.join(__dirname, "/../index.html"));
    });
};



module.exports = routesConfig;