using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using VMindbooke.SDK.Model;

namespace VMindbooke.RainbowKarma
{
    public class RainbowKarmaBot
    {
        private readonly RainbowClient _client;
        private readonly List<Post> _alreadyReposted;

        private readonly int _myLikeCountToStop;
        private readonly int _userLikeCountToRepost;
        private readonly int _postLikeCountToRepost;
        private readonly int _postLikeCountToComment;
        private readonly int _commentLikeCountToReply;

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

        private void MakeComments()
        {
            foreach (Post post in _client.GetPosts())
                if (post.Likes.Count >= _postLikeCountToComment)
                    _client.Comment(post);
        }

        private void MakeReplies()
        {
            foreach (Post post in _client.GetPosts())
                foreach (Comment comment in post.Comments)
                    if (comment.Likes.Count >= _commentLikeCountToReply)
                        _client.Reply(post, comment);
        }

        private void MakeReposts()
        {
            foreach (Post post in _client.GetPosts())
                if (post.Likes.Count >= _postLikeCountToRepost)
                    _client.Repost(post);
        }

        private void MakeTopReposts()
        {
            foreach (User user in _client.GetUsers())
                if (user.Likes.Count >= _userLikeCountToRepost)
                    _client.Repost(_client.GetUserPosts(user)
                        .OrderByDescending(post => post.Likes.Count).First());
        }

        public void Run()
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();
            RecurringJob.AddOrUpdate(() => MakeComments(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => MakeReplies(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => MakeReposts(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => MakeTopReposts(), Cron.Daily);
            
            using BackgroundJobServer backgroundJobServer = new BackgroundJobServer();
            Console.ReadKey();
        }
    }
}
