var googleAuth = require('./googleAuth');
var path = require('path');
var playlistsService = require('./youtube-playlists-service');

var routesConfig = {};

routesConfig.configure = function (app, port) {
    googleAuth.configure(port);

    app.get("/Authorize", (req, res) => {
        res.redirect(googleAuth.authorizationUrl);
    });

    app.get("/Authorized", (req, res) => {
        googleAuth.oauthClient.getToken(req.query.code, function (err, tokens) {
            if (err) {
                console.log(err);
                return;
            }
            googleAuth.oauthClient.setCredentials(tokens);
            res.redirect("/Playlists");
        });
    });

    app.get("/Playlists", (req, res) => {
        var result = playlistsService.getPlaylists(data => {
            res.send(JSON.stringify(data));
        });
    });

    app.get("/", (req, res) => {
        res.sendFile(path.join(__dirname, "/../index.html"));
    });
};

module.exports = routesConfig;