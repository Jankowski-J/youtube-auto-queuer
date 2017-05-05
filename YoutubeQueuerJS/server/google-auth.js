var google = require('googleapis');
var authConfig = require("./google-auth-config");
var path = require('path');
var fs = require('fs');

var authService = {};

var OAuth2 = google.auth.OAuth2;

var scopes = [
    'https://www.googleapis.com/auth/youtube',
    'https://www.googleapis.com/auth/youtube.force-ssl',
    'https://www.googleapis.com/auth/userinfo.email',
    'https://www.googleapis.com/auth/userinfo.profile'
];

authService.configure = function(port) {

    var oauth2Client = new OAuth2(
        authConfig.clientId,
        authConfig.clientSecret,
        `http://localhost:${port}/Authorized`
    );

    authService.authorizationUrl = oauth2Client.generateAuthUrl({
        access_type: 'offline',
        scope: scopes
    });

    authService.oauthClient = oauth2Client;
}

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
    var credentials = authService.credentials;
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
    authService.oauthClient.setCredentials(token);
}

function authorize(res, redirectUrl) {
    readToken()
        .then(token => {
            setSessionToken(token);
            res.redirect(redirectUrl)
        })
        .catch(error => {
            res.redirect(authService.authorizationUrl);
        });
}

function areSessionCredentialsValid() {
    var isTokenPresent = !!authService.oauthClient && !!authService.oauthClient.credentials
        && !!authService.oauthClient.credentials.access_token;

    if (!isTokenPresent) {
        return false;
    }

    var expiryDate = authService.oauthClient.credentials.expiry_date;
    var date = new Date(expiryDate);

    return date > new Date();
}

function areCredentialsValid(credentials) {
    var expiryDate = credentials.expiry_date;
    var date = new Date(expiryDate);

    return date > new Date();
}

function handleAuthorizationResponse(request, response) {
    authService.oauthClient.getToken(request.query.code,
        function(error, token) {
            if (error) {
                console.log(error);
                return;
            }

            storeToken(token);
            setSessionToken(token);
            response.redirect('/playlists');
        });
}

authService.middleware = function(req, res, next) {
    var isBeingAuthorized = req.originalUrl.indexOf('/Authorized') !== -1;

    if (isBeingAuthorized || areSessionCredentialsValid()) {
        next();
        return;
    }
    readToken().then(token => {
        if (token) {
            if(!areCredentialsValid(token)) {
                res.redirect(authService.authorizationUrl);
                return;
            }
            setSessionToken(token);
            next();
        }
    }).catch(error => {
        res.redirect(authService.authorizationUrl);
    });
}

authService.authorize = authorize;
authService.handleAuthorizationResponse = handleAuthorizationResponse;

module.exports = authService;