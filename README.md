# youtube-auto-queuer
A utility which lets you automatically add youtube videos to your playlist.

The idea for this tool appeared to me when I realized, that every other day, I add youtube videos to my watchlist. 
It seems like this task could be well automated, by making app which would fetch freshest video from my subscriptions and add them to my 
watchlist.

I aim this ASP.NET web app to be blueprint of that, implementing a "pull" model, in which I will trigger the refresh of my watchlist.
After I get familiar with the Youtube API, I may add a worker-like app, which will automatically trigger the addition of videos periodically.

