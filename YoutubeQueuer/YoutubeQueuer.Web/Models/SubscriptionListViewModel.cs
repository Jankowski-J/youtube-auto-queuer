using System.Collections.Generic;

namespace YoutubeQueuer.Web.Models
{
    public class SubscriptionListViewModel
    {
        public IEnumerable<SubscriptionModel> Subscriptions { get; set; }
    }
}