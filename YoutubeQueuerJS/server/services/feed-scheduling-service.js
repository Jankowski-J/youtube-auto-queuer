var path = require('path');
var fs = require('fs');

function getSettingsFilePath() {
    return path.join(process.env.APPDATA, '/scheduling_settings.json')
};

function readSettings() {
    var filePath = getSettingsFilePath();
    var promise = new Promise((resolve, reject) => {
        fs.readFile(filePath, (error, data) => {
            if (error) {
                if (error.code !== 'ENOENT') {
                    console.log('An error has occured when reading scheduling settings file:', error);
                }
                reject(error);
            }
            if (data) {
                var serialized = data.toString();
                var parsed = JSON.parse(serialized);
                resolve(parsed);
            }
        });
    });
    return promise;
}

function saveSettings(settings) {
    var filePath = getSettingsFilePath();
    var serialized = JSON.stringify(settings);
    fs.writeFile(filePath, serialized, error => {
        if (error) {
            console.log('An error has occured while saving scheduling settings:', error);
        }
    });
}

function saveFeedTimeForPlaylist(playlistId, scheduledTime) {
    var schedulingSettings = [];
    return readSettings()
        .then(settings => {
            schedulingSettings = settings;
        })
        .catch((error) => {
            if (error.code !== 'ENOENT') {
                throw error;
            }
        })
        .then(() => {
            var specificSetting = schedulingSettings
                .find(s => s.playlistId === playlistId);

            if (!specificSetting) {
                specificSetting = {
                    playlistId
                };
                schedulingSettings.push(specificSetting);
            }

            specificSetting.scheduledTime = scheduledTime;
            saveSettings(schedulingSettings);
        });
}

var feedSchedulingService = {
    saveFeedTimeForPlaylist: saveFeedTimeForPlaylist
};

module.exports = feedSchedulingService;