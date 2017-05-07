var schedulingUtils = require('../../server/scheduling/scheduling-utils');

describe('createScheduleTimesForHour for one schedule time', function() {
    it('should return correct date', function() {
        var hour = 12;
        var scheduledTime = schedulingUtils.createScheduleTimesForHour(hour, 1);

        var expected = (new Date()).addDays(1);
        expected.setHours(0, 0, 0, 0);
        expected = expected.addHours(hour);

        expect(scheduledTime[0].getTime()).toBe(expected.getTime());
    });
})

describe('createScheduleTimesForHour many one schedule time', function() {
    it('should return correct number of scheduled dates', function() {
        var hour = 12;
        var scheduledTime = schedulingUtils.createScheduleTimesForHour(hour, 5);

        var expected = 5;

        expect(scheduledTime.length).toBe(expected);
    });
})

describe('createScheduleTimesForHour many one schedule time', function() {
    it('should return correct scheduled dates', function() {
        var hour = 12;
        var scheduledTime = schedulingUtils.createScheduleTimesForHour(hour, 5);

        var firstDate = (new Date()).addDays(1);
        firstDate.setHours(0, 0, 0, 0);
        firstDate = firstDate.addHours(hour);

        var expected = [
            firstDate,
            firstDate.addDays(1),
            firstDate.addDays(2),
            firstDate.addDays(3),
            firstDate.addDays(4)
        ];

        expect(scheduledTime[0].getTime()).toBe(expected[0].getTime());
        expect(scheduledTime[1].getTime()).toBe(expected[1].getTime());
        expect(scheduledTime[2].getTime()).toBe(expected[2].getTime());
        expect(scheduledTime[3].getTime()).toBe(expected[3].getTime());
        expect(scheduledTime[4].getTime()).toBe(expected[4].getTime());
    });
})