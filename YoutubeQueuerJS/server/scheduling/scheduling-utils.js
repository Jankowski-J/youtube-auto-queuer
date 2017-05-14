var dateExtensions = require('../extensions/date-extensions');

function createScheduleTimesForHour(hour, schedulesCount) {
    var date = new Date();
    date.setHours(0, 0, 0, 0);
    date.addHours(hour);

    var dates = [];

    for(var iterator = 1; iterator <= 5; iterator++) {
        var scheduledDate = date.addDays(iterator);
        dates.push(scheduledDate);
    }
    return dates;
}

var schedulingUtils = {
    createScheduleTimesForHour: createScheduleTimesForHour
};

module.exports = schedulingUtils;