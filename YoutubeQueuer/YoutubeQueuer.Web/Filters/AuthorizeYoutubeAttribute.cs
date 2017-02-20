using System;

namespace YoutubeQueuer.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeYoutubeAttribute : Attribute
    {
    }
}