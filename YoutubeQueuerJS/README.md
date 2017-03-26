# YoutubeQueuerJS
This is a JavaScript implementation of a YoutubeQueuer functionality.

## Running this app

This app requires a `config.js file`, which provides a config object containing client data.
Repository contains a `config-sample.js` file, which is a template for this configuration file:

```javascript
var config = {};

config.clientId = "YOUR_CLIENT_ID";
config.clientSecret = "YOUR_CLIENT_ID";

module.exports = config;
```

In order to obtain client id and client secret you have to go to [Google developer console](https://console.developers.google.com/apis/credentials) and create an OAuth 2 client ID.

After you successfully created the `config.js` file, you have to run `npm start` in the app directory.