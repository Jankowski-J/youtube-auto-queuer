var googleAuth = require('./googleAuth');
var path = require('path');

var routesConfig = {};

routesConfig.configure = function (app, port) {
    googleAuth.configure(port);

    app.get("/Authorize", (req, res) => {
        res.redirect(googleAuth.authorizationUrl);
    });

    app.get("/Authorized", (req, res) => {
        res.redirect("/");
    });

    app.get("/", (req, res) => {
        res.sendFile(path.join(__dirname, "/../index.html"));
    });
};

module.exports = routesConfig;