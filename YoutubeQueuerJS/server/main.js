var config = require('./config');
var routes = require('./routes');
var express = require('express');
var path = require('path');

var app = express();

var scriptsPath = path.join(__dirname, '/../scripts');
app.use('/scripts', express.static(scriptsPath));

var port = config.port;
routes.configure(app, port);

app.listen(port, function () {
    console.log(`Youtube Queuer app listening on port ${port}!`)
});