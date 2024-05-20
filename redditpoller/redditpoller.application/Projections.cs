using Reddit.Controllers;
using redditpoller.application.Services.Models;

namespace redditpoller.application
{
    /// <summary>
    /// Projections to be used to convert objects of one type into another.
    /// </summary>
    public static class Projections
    {
        /// <summary>
        /// Converts a Post object from the Reddit.NET Nuget package into PostData for use in this application.
        /// </summary>
        /// <returns>Function representing projection.</returns>
        public static Func<Post, PostData> PostToPostData()
        {
            return post => new PostData
            {
                Author = post.Author,
                Title = post.Title,
                Url = Constants.RedditBaseUrl + post.Permalink,
                Fullname = post.Fullname,
                UpVotes = post.UpVotes,
                LastUpdated = DateTime.Now
            };
        }
    }
}
