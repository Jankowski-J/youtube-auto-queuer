var Services = Services || {};

Services.SubscriptionsService = function SubscriptionsService() {
    var that = this;
    that.getSubscriptions = function() {
        return fetch("/api/subscriptions")
            .then(response => response.json())
            .then(data => {
                for (let record of data) {
                    record.isIncluded = true;
                }

                return data;
            });
    };
    return that;
}

