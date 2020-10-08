using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Serilog;
using VmindbookeSDK;
using VMindbookeSDK.Entities;

namespace ShitBot
{
    public class ShitBotService
    {
        IEnumerable<Post> _posts;
        IEnumerable<User> _users;
        private readonly ShitBotClient _bot;
        private readonly int _comentaryBorder;
        private readonly int _replyBorder;
        private readonly int _repostBorder;
        private readonly int _userBorder;
        private readonly int _stopBorder;
        private readonly int _skip;
        private readonly int _take;
        private readonly VmindBookeClient _client;

        public ShitBotService()
        {
            IConfiguration cfg = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _bot = new ShitBotClient();
            _client = new VmindBookeClient(cfg["ApiBaseUrl"]);
            RefreshData();
            _comentaryBorder = Int32.Parse(cfg["_commentaryBorder"]);
            _replyBorder = Int32.Parse(cfg["_replyBorder"]);
            _repostBorder = Int32.Parse(cfg["_repostBorder"]);
            _userBorder = Int32.Parse(cfg["_userBorder"]);
            _stopBorder = Int32.Parse(cfg["_stopBorder"]);
            _skip = Int32.Parse(cfg["skip"]);
            _take = Int32.Parse(cfg["take"]);
        }

        public void RefreshData()
        {
            
            _posts = _client.GetPosts(_skip, _take);
            _users = _client.GetUsers(_skip, _take);
        }

        public void CommentOnPopularPosts()
        {
            foreach (var post in _posts)
            {
                if (post.Likes.Count() >= _comentaryBorder)
                {
                    _bot.Comment(post);
                }
            }
            Log.Logger.Information("CommentOnPopularPosts Done");
        }
        
        public void ReplyOnPopularComment()
        {
            foreach (var post in _posts)
            {
                foreach (var comment in post.Comments)
                {
                    if (comment.Likes.Count() >= _replyBorder)
                    {
                        _bot.Reply(post,comment);
                    }
                }
            }
            Log.Logger.Information("ReplyOnPopularComment Done");
        }
        
        public void RepostOfPopularPost()
        {
            foreach (var post in _posts)
            {
                if (post.Likes.Count() >= _repostBorder)
                {
                    _bot.Repost(post);
                }
            }
            Log.Logger.Information("RepostOfPopularPost Done");
        }
        
        public void StealPopularUsersPost()
        {
            foreach (var user in _users)
            {
                var likes_amount = _posts.Where(post => post.AuthorId == user.Id)
                      .Sum(i => i.Likes.Count());
                if (likes_amount >= _userBorder)
                {
                    var best_post_likes_amount = _posts
                        .Where(post => post.AuthorId == user.Id)
                        .Max(i => i.Likes.Count());
                    var best_post = _posts
                        .First(post => post.Likes.Count() == best_post_likes_amount);
                    _bot.Repost(best_post);
                }
            }
            Log.Logger.Information("StealPopularUsersPost Done");
        }

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        public bool IsTimeToStop()
        {
            var likes_amount = _posts.Where(post => post.AuthorId == _bot._credentials.Id)
                    .Sum(i => i.Likes
                    .Count(like => like.PlacingDateUtc.Day == DateTime.Now.Day));
            return likes_amount >= _stopBorder;
        }

        public void StartFarming()
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();
            RecurringJob.AddOrUpdate(()=>CommentOnPopularPosts(),Cron.Minutely);
            RecurringJob.AddOrUpdate(()=>ReplyOnPopularComment(),Cron.Minutely);
            RecurringJob.AddOrUpdate(()=>RepostOfPopularPost(),Cron.Minutely);
            RecurringJob.AddOrUpdate(()=>StealPopularUsersPost(),Cron.Minutely);
            RecurringJob.AddOrUpdate(()=>RefreshData(),Cron.Hourly);
           
            using (var backgroundServer = new BackgroundJobServer())
            {
                int i = 0;
                Log.Logger.Information("Job Started");
                while (!IsTimeToStop())
                {
                    Thread.Sleep(60000);
                }
                Log.Logger.Information("Job Done");
            }
        }
    }
}