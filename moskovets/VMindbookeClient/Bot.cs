using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ET.FakeText;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace VMindbookeClient
{
    public class BotStarter
    {
        private BackgroundJobServer _server;

        public BotStarter()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.File("logger.log", restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();
            Log.Logger = logger;

            GlobalConfiguration.Configuration.UseMemoryStorage();

            _server = new BackgroundJobServer();
        }

        public void Start()
        {
            Bot.Start();
        }

        ~BotStarter()
        {
            _server.Dispose();
        }

    }
    
    public class Bot
    {
        private UserAuthInfo _user;
        private IApiClient _client;

        private int _countUserLikes = 5;
        private int _countPostLikesForComment = 5;
        private int _countPostLikesForCopy = 5;
        private int _countUserLikesForCopy = 5;
        private int _countCommentLikesForReply = 5;
        private int _loadStep = 5;

        private static HashSet<int> _commentedPosts = new HashSet<int>();
        private static HashSet<int> _copiedPosts = new HashSet<int>();
        private static HashSet<int> _copiedUsers = new HashSet<int>();
        private static HashSet<string> _repliedComments = new HashSet<string>();

        private static Random _rand = new Random();
        

        Bot()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _user = new UserAuthInfo(configuration["bot:userName"],
                Int32.Parse(configuration["bot:userId"]),
                configuration["bot:userToken"]);
            _client = new ApiClient(configuration["api:address"]);
            
        }
        
        public bool CheckStopCondition()
        {
            try
            {
                var user = _client.GetUser(_user.Id);
                return user.Likes.Count >= _countUserLikes;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public int CountLikesForToday(IReadOnlyCollection<Like> likes)
        {
            return likes.Count(l => l.PostingDateUtc.Date == DateTime.Today);
        }

        public static string CreateRandomContent()
        {
            var generator = new TextGenerator();
            var text = generator.GenerateText(_rand.Next(2, 10));
            return text;
        }

        public void DoWritingComment(IReadOnlyCollection<Post> posts)
        {
            foreach (var post in posts)
            {
                if (CountLikesForToday(post.Likes) > _countPostLikesForComment
                    && !_commentedPosts.Contains(post.Id))
                {
                    _client.CreateComment(_user, post.Id, CreateRandomContent());
                    _commentedPosts.Add(post.Id);
                }
            }
        }

        public void DoCopingPost(IReadOnlyCollection<Post> posts)
        {
            foreach (var post in posts)
            {
                if (CountLikesForToday(post.Likes) > _countPostLikesForCopy
                    && !_copiedPosts.Contains(post.Id))
                {
                    _client.CreatePost(_user, post.Content, post.Title);
                    _copiedPosts.Add(post.Id);
                }
            }
        }

        public void DoReplyComment(IReadOnlyCollection<Post> posts)
        {
            foreach (var post in posts)
            {
                foreach (var comment in post.Comments)
                {
                    if (CountLikesForToday(comment.Likes) > _countCommentLikesForReply
                        && !_repliedComments.Contains(comment.Id))
                    {
                        _client.ReplyComment(_user, post.Id, comment.Id, CreateRandomContent());
                        _repliedComments.Add(comment.Id);
                    }
                }
            }
        }

        public void DoCopySuccessPost(IReadOnlyCollection<User> users)
        {
            foreach (var user in users)
            {
                if (CountLikesForToday(user.Likes) > _countUserLikesForCopy &&
                    !_copiedUsers.Contains(user.Id) && user.Id != _user.Id)
                {
                    var posts = _client.GetUserPosts(user.Id);
                    var bestPost = posts.First(post => post.Likes.Length == posts.Max(p => p.Likes.Length));
                    if (!_copiedPosts.Contains(bestPost.Id))
                    {
                        _client.CreatePost(_user, bestPost.Content, bestPost.Title);
                        _copiedPosts.Add(bestPost.Id);
                        _copiedUsers.Add(user.Id);
                    }
                }
            }
        }

        public void LoadAndProcessing<T>(Action<IReadOnlyCollection<T>> work,
            Func<int, int, IReadOnlyCollection<T>> getter)
        {
            int take = _loadStep;
            int skip = 0;
            IReadOnlyCollection<T> entities = new List<T>();
            do
            {
                try
                {
                    entities = getter(take, skip);
                    skip += take;
                    work(entities);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            } while (entities.Count == take);
        }

        public void WriteCommentsToPosts()
        {
            Log.Information("Start WriteCommentsToPosts");
            LoadAndProcessing(DoWritingComment, _client.GetPosts);
        }

        public void WriteRepliesToComments()
        {
            Log.Information("Start WriteRepliesToComments");
            LoadAndProcessing(DoReplyComment, _client.GetPosts);
        }

        public void CopySuccessfulPosts()
        {
            Log.Information("Start CopySuccessfulPosts");
            LoadAndProcessing(DoCopingPost, _client.GetPosts);
        }

        public void CopySuccessfulPostsFromSuccessfulUsers()
        {
            Log.Information("Start CopySuccessfulPostsFromSuccessfulUsers");
            LoadAndProcessing(DoCopySuccessPost, _client.GetUsers);
        }

        private static List<string> _jobs;
        static public void Start()
        {
            DateTime now = DateTime.Now;
            DateTime tomorrow = now.AddDays(1).Date;
            BackgroundJob.Schedule(() => Bot.Start(), tomorrow - now);
            if (_jobs.Count > 0)
                Stop();
            for (int i = 0; i < 5; i++)
                _jobs.Add(Guid.NewGuid().ToString("N"));
            RecurringJob.AddOrUpdate<Bot>(_jobs[0], (bot) => bot.CopySuccessfulPostsFromSuccessfulUsers(), Cron.Minutely);
            RecurringJob.AddOrUpdate<Bot>(_jobs[1], (bot) => bot.CopySuccessfulPosts(), Cron.Minutely);
            RecurringJob.AddOrUpdate<Bot>(_jobs[2], (bot) => bot.WriteRepliesToComments(), Cron.Minutely);
            RecurringJob.AddOrUpdate<Bot>(_jobs[3], (bot) => bot.WriteCommentsToPosts(), Cron.Minutely);
            RecurringJob.AddOrUpdate<Bot>(_jobs[4], (bot) => bot.WriteCommentsToPosts(), Cron.Minutely);
        }

        public void CheckAndStopServer()
        {
            if (CheckStopCondition())
                Stop();
        }
        public static void Stop()
        {
            foreach (var job in _jobs)
            {
                RecurringJob.RemoveIfExists(job);
            }
            _jobs.Clear();
        }

        public void Run()
        {
            BackgroundJob.Schedule(() => Bot.Start(), TimeSpan.Zero);
        }
    }
}