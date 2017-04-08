var Services = Services || {};

Services.SubscriptionsService = function SubscriptionsService() {
    var that = this;
    that.getSubscriptions = function(callback) {
        callback = callback || function() { };
        return fetch("/api/subscriptions")
            .then(response => response.json())
            .then(data => callback(data));
    };
    return that;
}

