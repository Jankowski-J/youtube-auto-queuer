var google = require('googleapis');
var authConfig = require("./google-auth-config");

var authService = {};

var OAuth2 = google.auth.OAuth2;

var scopes = [
    'https://www.googleapis.com/auth/youtube',
    'https://www.googleapis.com/auth/youtube.force-ssl'
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

authService.middleware = function (req, res, next) {
    // TODO: implement correct authorization middleware
    next();
}

module.exports = authService;