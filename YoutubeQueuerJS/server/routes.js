var googleAuth = require('./google-auth');
var path = require('path');
var fs = require('fs');
var crypto = require('crypto');

var playlistsService = require('./services/youtube-playlists-service');
var subscriptionsService = require('./services/youtube-subscriptions-service');
var videosService = require('./services/youtube-videos-service');

var routesConfig = {};

var TOKEN_DIR = path.join(process.env.APPDATA, '/.credentials/');
var TOKEN_PATH = path.join(TOKEN_DIR, 'youtube_queuer_js.json');

function ensureTokenDirExists() {
    var promise = new Promise((resolve, reject) => {
        fs.exists(TOKEN_DIR, exists => {
            if (!exists) {
                fs.mkdir(TOKEN_DIR);
            }

            resolve();
        });
    });
    return promise;
}

function storeToken(token) {
    ensureTokenDirExists().then(() => {
        fs.writeFile(TOKEN_PATH, JSON.stringify(token), (error, data) => {
            if (error) {
                console.log('Error while storing token:', error);
            }
            if (data) {
                console.log('Token stored to ' + TOKEN_PATH);
            }
        });
    });
}

function readToken() {
    var promise = new Promise((resolve, reject) => {
        fs.readFile(TOKEN_PATH, (error, data) => {
            if (error) {
                console.error('Error while reading token from file:', error);
                reject(error);
            }
            if (data && data.length > 0) {
                var serialized = data.toString();
                var parsed = JSON.parse(serialized);
                resolve(parsed);
            }
            reject("Invalid authorization token");
        });
    });

    return promise;
}

function getCredentialsHash() {
    var credentials = googleAuth.oauthClient.credentials;
    var stringified = JSON.stringify(credentials);
    var hash = crypto.createHash('sha256')
        .update(stringified)
        .digest('base64');

    return hash;
}

function setAuthorizedCookie(obj) {
    var hashed = getCredentialsHash();
    obj.cookie('Authorization', hashed);
}

function setSessionToken(token) {
    googleAuth.oauthClient.setCredentials(token);
}

function authorize(res, redirectUrl, next) {
    var token = readToken()
        .then(token => {
            setSessionToken(token);
            res.redirect(redirectUrl)
        })
        .catch(error => {
            res.redirect(googleAuth.authorizationUrl);
        });
}

function setupAuthorizationUrls(app) {
    app.get('/authorize', (req, res) => {
        authorize(res, '/playlists');
    });

    app.get('/authorized', (req, res) => {
        googleAuth.oauthClient.getToken(req.query.code,
            function(error, token) {
                if (error) {
                    console.log(error);
                    return;
                }

                storeToken(token);
                setSessionToken(token);
                res.redirect('/playlists');
            });
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

        subscriptionsService.getSubscriptions()
            .then(subs => {
                var validSubs = subs.filter(s => s.isIncluded);

                var promises = validSubs.map(sub => {
                    return function() {
                        return videosService.getLatestVideosFromChannel(sub.channelId)
                            .then(videos => {
                                var ids = videos.map(v => v.id);

                                playlistsService.addVideosToPlaylist(ids, playlistId);
                            })
                            .catch(error =>
                                console.log('An error occured while getting videos from channel: ' + error));
                    }
                });

                promises.reduce((cur, next) => {
                    return cur.then(next);
                }, Promise.resolve())
                    .then(() => { });
            });
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