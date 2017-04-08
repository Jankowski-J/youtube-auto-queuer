var googleAuth = require('./googleAuth');
var path = require('path');
var playlistsService = require('./services/youtube-playlists-service');
var subscriptionsService = require('./services/youtube-subscriptions-service');
var fs = require('fs');

var routesConfig = {};

var TOKEN_DIR = path.join((process.env.HOME || process.env.HOMEPATH ||
    process.env.USERPROFILE), '/.credentials/');
var TOKEN_PATH = path.join(TOKEN_DIR + 'youtube_queuer_js.json');

function storeToken(token) {
    try {
        fs.mkdir(TOKEN_DIR);
    }
    catch (error) {
        if (error.code != 'EEXIST') {
            throw error;
        }
    }

    fs.writeFile(TOKEN_PATH, JSON.stringify(token));
    console.log('Token stored to ' + TOKEN_PATH);
}

function setupAuthorizationUrls(app) {
    app.get("/authorize", (req, res) => {
        fs.readFile(TOKEN_PATH, function(error, token) {
            if (error) {
                res.redirect(googleAuth.authorizationUrl);
            } else {
                googleAuth.oauthClient.credentials = JSON.parse(token);
                res.redirect("/subscriptions");
            }
        });
    });

    app.get("/authorized", (req, res) => {
        googleAuth.oauthClient.getToken(req.query.code,
            function(error, tokens) {
                if (error) {
                    console.log(error);
                    return;
                }
                googleAuth.oauthClient.setCredentials(tokens);
                storeToken(tokens);

                res.redirect("/subscriptions");
            });
    });
}

function setupApi(app) {
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