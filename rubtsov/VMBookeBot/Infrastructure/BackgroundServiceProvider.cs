using Microsoft.Extensions.Configuration;
using VMBook.SDK;
using VMBookeBot.Domain;

namespace VMBookeBot.Infrastructure
{
    public class BackgroundServiceProvider
    {
        private readonly BotThiefService _botThiefService;

        public BackgroundServiceProvider() : this(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build())
        {
        }

        public BackgroundServiceProvider(IConfigurationRoot configuration)
        {
            var currentUserId = int.Parse(configuration["currentUserId"]);
            var vmbClient = new VmbClient(configuration["serverUrl"],
                currentUserId,
                configuration["authorizationToken"]);
            var requestsRetryer = new RequestsRetryer(vmbClient);
            var collectionsFilter = new RequestsFilter(vmbClient);
            var botThief = new BotThief(requestsRetryer, collectionsFilter, RepositoryLoader.Load());
            _botThiefService = new BotThiefService(botThief, collectionsFilter, configuration);
        }

        public void WriteCommentService()
        {
            _botThiefService.WriteCommentService();
        }

        public void WriteReplyService()
        {
            _botThiefService.WriteReplyService();
        }

        public void StealPopularPostService()
        {
            _botThiefService.StealPopularPostService();
        }

        public void StealPostFromPopularUserService()
        {
            _botThiefService.StealPostFromPopularUserService();
        }
    }
}