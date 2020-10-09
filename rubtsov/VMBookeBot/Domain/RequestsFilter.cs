using System.Collections.Generic;
using System.Linq;
using Serilog;
using VMBook.SDK;

namespace VMBookeBot.Domain
{
    public class RequestsFilter : IRequestFilter
    {
        private readonly IVmbClientFilter _vmbClient;

        public RequestsFilter(IVmbClientFilter vmbClient)
        {
            _vmbClient = vmbClient;
        }
        
        public User GetUser(int id)
        {
            return _vmbClient.GetUser(id);
        }
        public IReadOnlyCollection<Post> GetPostsWithMinimumLikesNumber(int minLikesNumber)
        {
            Log.Information($"Requesting posts with likes number >= {minLikesNumber}");
            return _vmbClient.GetPosts().Where(post => post.Likes.Count >= minLikesNumber).ToArray();
        }

        public IReadOnlyCollection<User> GetPopularUsers(int minLikesNumber)
        {
            Log.Information($"Requesting users with likes number >= {minLikesNumber}");
            return _vmbClient.GetUsers().Where(user => user.Likes
                .GroupBy(like => like.PlacingDateUtc.DayOfYear)
                .Any(groupOfLikesByDay => groupOfLikesByDay.Count() >= minLikesNumber)).ToArray();
        }
        
        public Post GetMostPopularUserPost(int userId)
        {
            Log.Information($"Looking for the most popular post of user with id: {userId}");
            var clientRequestResult = _vmbClient.GetUserPosts(userId);
            var mostPopularPost = clientRequestResult.OrderByDescending(post => post.Likes.Count()).First();
            Log.Information($"The most popular post was found with post id: {mostPopularPost.Id}");
            return mostPopularPost;
        }
    }
}