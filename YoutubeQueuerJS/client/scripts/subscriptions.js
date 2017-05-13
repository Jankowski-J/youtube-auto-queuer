var service = new Services.SubscriptionsService();
var viewModel = {};
var subs = service.getSubscriptions()
    .then(subs => {
        viewModel = {
            subscriptions: subs
        };
        ko.applyBindings(viewModel);
    });

function saveSubscriptions() {
    var data = JSON.stringify(viewModel);

    var headers = new Headers();
    headers.append("Content-Type", "application/json");

    fetch("/api/subscriptions", {
        method: "POST",
        body: data,
        headers
    }).catch(error => {
        console.log("Error while saving subscriptions:", error);
    });
}