using Microsoft.Extensions.Configuration;
using Serilog;

namespace VMBookeBot.Domain
{
    public class BotThiefService
    {
        private readonly BotThief _botThief;
        private readonly IConfigurationRoot _configuration;
        private readonly RequestsFilter _requestsFilter;
        public BotThiefService(
            BotThief botThief, 
            RequestsFilter requestsFilter,
            IConfigurationRoot configuration
            )
        {
            _botThief = botThief;
            _configuration = configuration;
            _requestsFilter = requestsFilter;
        }
        public void WriteCommentService()
        {
            //TODO: отлавливать исключения в случае неудачного парсинга
            var minLikesNumber = int.Parse(_configuration["minLikesForNewComment"]);
            Log.Information("Starting to write comments under posts" +
                            $" where number of likes >= {minLikesNumber}");
            var posts = _requestsFilter.GetPostsWithMinimumLikesNumber(minLikesNumber);
            _botThief.WriteCommentUnderPopularPosts(posts);
            Log.Information("Writing comments task completed");
        }

        public void WriteReplyService()
        {
            var minLikesNumber = int.Parse(_configuration["minLikesForNewReply"]);
            Log.Information("Starting to write replies under comments" +
                            $" where number of likes >= {minLikesNumber}");
            var posts = _requestsFilter.GetPostsWithMinimumLikesNumber(0);
            _botThief.WriteReplyUnderPopularComment(posts, minLikesNumber);
            Log.Information("Writing replies task completed");
        }

        public void StealPopularPostService()
        {
            var minLikesNumber = int.Parse(_configuration["minLikesForNewPost"]);
            Log.Information("Starting to steal popular posts" +
                            $"where likes number >= {minLikesNumber}");
            var posts = _requestsFilter.GetPostsWithMinimumLikesNumber(0);
            _botThief.StealPopularPost(posts, minLikesNumber);
            Log.Information("Stealing posts task completed");
        }

        public void StealPostFromPopularUserService()
        {
            var minLikesNumber = int.Parse(_configuration["minLikesPerDayToStealBestPost"]);
            var popularUsers = _requestsFilter.GetPopularUsers(minLikesNumber);

            Log.Information("Starting to look for users" +
                            $"whose daily likes number >= {minLikesNumber}");
            _botThief.StealPostFromPopularUser(popularUsers);
            Log.Information("Stealing posts from popular users task completed");
        }

        public bool CurrentUserLikesLimitCheckService(int currentUserId)
        {
            var minLikesNumber = int.Parse(_configuration["minLikesForCurrentUser"]);
            return _botThief.IsCurrentUserLikesLimitExceeded(currentUserId, minLikesNumber);
        }
    }
}