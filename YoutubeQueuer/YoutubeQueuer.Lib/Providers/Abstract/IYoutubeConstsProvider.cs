namespace YoutubeQueuer.Lib.Providers.Abstract
{
    public interface IYoutubeConstsProvider
    {
        string PlaylistsListParts { get; }
        string InsertPlaylistItemsParts { get; }
        string YoutubePlaylistItemKind { get; }
        string VideoResourceKind { get; }
        string ChannelsListParts { get; }
        string SubscriptionsListParts { get; }
        string VideosListParts { get; }
        int MaxResultsCount { get; }
    }
}