using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Serilog;
using VMindbooke.SDK.Model;

namespace VMindbooke.RainbowKarma
{
    public class RainbowKarmaBot
    {
        private readonly List<Post> _alreadyReposted;
        private readonly IRainbowClient _client;
        private readonly int _commentLikeCountToReply;

        private readonly int _myLikeCountToStop;
        private readonly int _postLikeCountToComment;
        private readonly int _postLikeCountToRepost;
        private readonly int _userLikeCountToRepost;

        public RainbowKarmaBot()
        {
            _client = new RainbowClient();
            IConfiguration cfg = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();
            
            _myLikeCountToStop = int.Parse(cfg["MyLikeCountToStop"]);
            _userLikeCountToRepost = int.Parse(cfg["UserLikeCountToRepost"]);
            _postLikeCountToRepost = int.Parse(cfg["PostLikeCountToRepost"]);
            _postLikeCountToComment = int.Parse(cfg["PostLikeCountToComment"]);
            _commentLikeCountToReply = int.Parse(cfg["CommentLikeCountToReply"]);
        }

        public void MakeComments()
        {
            foreach (Post post in _client.GetPosts())
            {
                Log.Information($"comments: {post.Title}");
                if (post.Likes.Count >= _postLikeCountToComment)
                    _client.Comment(post);
            }
        }

        public void MakeReplies()
        {
            foreach (Post post in _client.GetPosts())
            {
                Log.Information($"replies: {post.Title}");
                foreach (Comment comment in post.Comments)
                    if (comment.Likes.Count >= _commentLikeCountToReply)
                        _client.Reply(post, comment);
            }
        }

        public void MakeReposts()
        {
            foreach (Post post in _client.GetPosts())
            {
                Log.Information($"reposts: {post.Title}");
                if (post.Likes.Count >= _postLikeCountToRepost)
                    _client.Repost(post);
            }
        }

        public void MakeTopReposts()
        {
            foreach (User user in _client.GetUsers())
            {
                Log.Information($"topReposts: {user.Name}");
                if (user.Likes.Count >= _userLikeCountToRepost)
                    _client.Repost(_client.GetUserPosts(user)
                        .OrderByDescending(post => post.Likes.Count).First());
            }
        }

        public void Run()
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();

            MakeComments();
            MakeReplies();
            MakeReposts();
            MakeTopReposts();
            /*
            RecurringJob.AddOrUpdate(() => MakeComments(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => MakeReplies(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => MakeReposts(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => MakeTopReposts(), Cron.Daily);
            */
            
            using BackgroundJobServer backgroundJobServer = new BackgroundJobServer();
            Console.ReadKey();
        }
    }
}
