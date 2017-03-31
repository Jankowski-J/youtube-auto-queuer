var googleAuth = require('./googleAuth');
var path = require('path');
var playlistsService = require('./youtube-playlists-service');
var subscriptionsService = require('./youtube-subscriptions-service');

var routesConfig = {};

routesConfig.configure = function (app, port) {
    googleAuth.configure(port);

    app.get("/authorize", (req, res) => {
        res.redirect(googleAuth.authorizationUrl);
    });

    app.get("/authorized", (req, res) => {
        googleAuth.oauthClient.getToken(req.query.code, function (err, tokens) {
            if (err) {
                console.log(err);
                return;
            }
            googleAuth.oauthClient.setCredentials(tokens);
            res.redirect("/playlists");
        });
    });

    app.get("/api/playlists", (req, res) => {
        var result = playlistsService.getPlaylists(data => {
            res.send(JSON.stringify(data));
        });
    });

    app.get("/api/subscriptions", (req, res) => {
        subscriptionsService.getSubscriptions(data =>{
            res.send(JSON.stringify(data));
        });
    });

    app.get("/", (req, res) => {
        res.sendFile(path.join(__dirname, "/../index.html"));
    });
};

module.exports = routesConfig;