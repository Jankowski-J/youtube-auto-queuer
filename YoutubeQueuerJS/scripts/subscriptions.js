var SubscriptionsService = (function() {
    var SubscriptionsService = function() {
        var that = this;
        that.getSubscriptions = function(callback) {
            callback = callback || function() { };
            return fetch("/api/subscriptions")
                .then(response => response.json())
                .then(data => callback(data));
        };
    }

    return SubscriptionsService;
}());
