using System.Collections.Generic;

namespace YoutubeQueuer.Web.Models
{
    public class SubscriptionListViewModel
    {
        public IEnumerable<YoutubeSubscriptionWebModel> Subscriptions { get; set; }
    }
}