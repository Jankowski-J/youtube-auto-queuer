var googleAuth = require('./googleAuth');
var path = require('path');
var fs = require('fs');
var crypto = require('crypto');

var playlistsService = require('./services/youtube-playlists-service');
var subscriptionsService = require('./services/youtube-subscriptions-service');
var videosService = require('./services/youtube-videos-service');

var routesConfig = {};

var TOKEN_DIR = path.join((process.env.HOME || process.env.HOMEPATH ||
    process.env.USERPROFILE), '/.credentials/');
var TOKEN_PATH = path.join(TOKEN_DIR + 'youtube_queuer_js.json');

function storeToken(token) {
    // try {
    //     fs.mkdir(TOKEN_DIR);
    // }
    // catch (error) {
    //     console.log(error);
    //     if (error.code != 'EEXIST') {
    //         throw error;
    //     }
    // }

    // fs.writeFile(TOKEN_PATH, JSON.stringify(token));
    // console.log('Token stored to ' + TOKEN_PATH);
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

function authorize(res, redirectUrl, next) {
    // redirectUrl = redirectUrl || '/subscriptions';
    // fs.readFile(TOKEN_PATH, function(error, token) {
    //     if (error) {
    res.redirect(googleAuth.authorizationUrl);
    //     } else {
    //         googleAuth.oauthClient.credentials = JSON.parse(token);
    //         setAuthorizedCookie(res);
    //         if (next) {
    //             next();
    //         }
    //         res.redirect(redirectUrl);
    //     }
    // });
}

function setupAuthorizationUrls(app) {
    app.get("/authorize", (req, res) => {
        authorize(res);
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
                setAuthorizedCookie(res);
                res.redirect('/playlists');
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

    app.post("/api/feedvideos", (req, res) => {
        var playlistId = req.body.playlistId;

        subscriptionsService.getSubscriptions()
            .then(subs => {
                for (let sub of subs) {
                    videosService.getLatestVideosFromChannel(sub.channelId)
                        .then(videos => {
                            var ids = videos.map(v => v.id);

                            playlistsService.addVideosToPlaylist(ids, playlistId);
                        });
                }
            });
    })
}

function setupViews(app) {
    app.get("/subscriptions", (req, res) => {
        res.sendFile(path.join(__dirname, "/../views/subscriptions.html"));
    });

    app.get("/playlists", (req, res) => {
        res.sendFile(path.join(__dirname + "/../views/playlists.html"));
    });
}

function authorizationMiddleware(req, res, next) {
    console.log(req.cookies, res.cookies);
    next();
}

routesConfig.configure = function(app, port) {
    googleAuth.configure(port);

    app.use(authorizationMiddleware);
    setupAuthorizationUrls(app);
    setupApi(app);
    setupViews(app);

    app.get("/", (req, res) => {
        res.sendFile(path.join(__dirname, "/../index.html"));
    });
};



module.exports = routesConfig;