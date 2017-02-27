namespace YoutubeQueuer.Web.Models
{
    public class YoutubeSubscriptionWebModel
    {
        public string ChannelName { get; set; }
        public string ChannelId { get; set; }
        public YoutubeVideoWebModel LatestVideo { get; set; }
    }
}