using YoutubeQueuer.Lib.Providers.Abstract;

namespace YoutubeQueuer.Lib.Providers
{
    internal class StaticYoutubeConstsProvider : IYoutubeConstsProvider
    {
        public string PlaylistsListParts => "id,snippet,contentDetails";
        public string InsertPlaylistItemsParts => "id,snippet";
        public string YoutubePlaylistItemKind => "youtube#playlistIem";
        public string VideoResourceKind => "youtube#video";
        public string ChannelsListParts => "id,snippet,contentDetails";
        public string SubscriptionsListParts => "id,snippet,contentDetails";
        public string VideosListParts => "id,snippet";
        public int MaxResultsCount => 50;
    }
}
