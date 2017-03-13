namespace YoutubeQueuer.Lib.Models
{
    public class UserSubscriptionModel : YoutubeSubscriptionModel
    {
        public string UserName { get; set; }
        public bool IsIncluded { get; set; }
    }
}
