var google = require('googleapis');
var config = require("./config");
var express = require("express");
var app = express();

var util = require("util");

const port = 8080;

var OAuth2 = google.auth.OAuth2;

var oauth2Client = new OAuth2(
    config.clientId,
    config.clientSecret,
    `http://localhost:${port}/Authorized`
);

var scopes = [
    'https://www.googleapis.com/auth/youtube',
    'https://www.googleapis.com/auth/youtube.force-ssl'
];

var url = oauth2Client.generateAuthUrl({
    access_type: 'offline',
    scope: scopes
});


app.get("/Authorize", (req, res) => {
    console.log(url);
    res.redirect(url);
});

app.get("/Authorized", (req, res) => {
    res.redirect("/");
});

app.get("/", (req, res) => {
    res.send("benito");
});

app.listen(port, function () {
    console.log(`Example app listening on port ${port}!`)
})