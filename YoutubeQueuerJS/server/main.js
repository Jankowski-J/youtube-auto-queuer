var config = require('./config');
var routes = require('./routes');
var express = require('express');
var path = require('path');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');

var app = express();
app.use(cookieParser());
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

var scriptsPath = path.join(__dirname, '/../client/scripts');
app.use('/scripts', express.static(scriptsPath));

var port = config.port;
routes.configure(app, port);

app.listen(port, function() {
    console.log(`Youtube Queuer app listening on port ${port}!`)
});