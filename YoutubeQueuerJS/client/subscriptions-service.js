var Services = Services || {};

Services.SubscriptionsService = function SubscriptionsService() {
    var that = this;
    that.getSubscriptions = function() {
        return fetch("/api/subscriptions")
            .then(response => response.json());
    };
    return that;
}

