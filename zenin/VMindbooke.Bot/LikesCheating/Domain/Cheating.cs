using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using VMindbooke.SDK;
using Serilog;

namespace LikesCheating.Domain
{
    public class Cheating : ICheating
    {
        public Cheating()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("actions.log")
                .WriteTo.Console()
                .CreateLogger();
            
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _client = new VMindbookeClient(configuration["VMindbookeUrl"]);
            _thresholds = new Thresholds(
                Convert.ToInt32(configuration["PostThresholdToComment"]),
                Convert.ToInt32(configuration["PostThresholdToDuplicate"]),
                Convert.ToInt32(configuration["CommentThresholdToReply"]),
                Convert.ToInt32(configuration["UserSuccessPostThreshold"]),
            Convert.ToInt32(configuration["UserTargetThreshold"])
            );
        }
        public void CommentIfLikesMoreThanThreshold(string token)
        {
            var posts = _client.GetPosts();
            foreach (var post in posts)
            {
                if (post.Likes.Where(like => like.PlacingDateUtc < DateTime.Today).Count() > 
                    _thresholds.PostThresholdToComment 
                    && !_commentedPosts.Contains(post.Id))
                {
                    var commentContent = new CommentContent("Hello there! I'm using likes cheating");
                    _client.Comment(commentContent, post.Id, token);
                    _commentedPosts.Add(post.Id);
                    Log.Information($"Comment to post {post.Id} added");
                }
            }
        }

        public void ReplyIfLikesMoreThanThreshold(string token)
        {
            var posts = _client.GetPosts();
            foreach (var post in posts)
            {
                foreach (var comment in post.Comments)
                {
                    if (comment.Likes.Where(like => like.PlacingDateUtc < DateTime.Today).Count() >
                        _thresholds.CommentThresholdToReply
                        && !_repliedComments.Contains(comment.Id))
                    {
                        var replyContent = new ReplyContent("Hello there! I'm using likes cheating");
                        _client.Reply(replyContent, post.Id, comment.Id, token);
                        _repliedComments.Add(comment.Id);
                        Log.Information($"Reply to comment {comment.Id} added");
                    }
                }
            }
        }

        public void DuplicatePostIfLikesMoreThanThreshold(int userId, string token)
        {
            var posts = _client.GetPosts();
            foreach (var post in posts)
            {
                if (post.Likes.Where(like => like.PlacingDateUtc < DateTime.Today).Count() >
                    _thresholds.PostThresholdToDuplicate
                    && !_duplicatedPosts.Contains(post.Id))
                {
                    var postContent = new PostContent("Spizjeno", post.Content);
                    _client.Post(postContent, userId, token);
                    _duplicatedPosts.Add(post.Id);
                    Log.Information($"Post {post.Id} duplicated");
                }
            }
        }

        public void DuplicateMostSuccessfulPostIfLikesMoreThanThreshold(int userId, string token)
        {
            var users = _client.GetUsers();
            foreach (var user in users)
            {
                var userPosts = _client.GetUserPosts(user.Id);
                foreach (var post in userPosts)
                {
                    if (post.Likes.Where(like => like.PlacingDateUtc < DateTime.Today).Count() > 
                        _thresholds.UserSuccessPostThreshold
                        && !_duplicatedPosts.Contains(post.Id))
                    {
                        var postContent = new PostContent(post.Title, post.Content);
                        _client.Post(postContent, userId, token);
                        _duplicatedPosts.Add(post.Id);
                        Log.Information($"Post {post.Id} duplicated");
                    }
                }
            }
        }

        public bool CheckUserReachedLikesThreshold(int userId)
        {
            var user = _client.GetUser(userId);
            Log.Information($"User {userId} has {user.Likes.Count} likes");
            if (user.Likes.Count < _thresholds.UserDailyTargetThreshold)
                return false;
            return true;
        }

        private readonly Thresholds _thresholds;
        private readonly VMindbookeClient _client;
        private readonly List<int> _commentedPosts = new List<int>();
        private readonly List<int> _duplicatedPosts = new List<int>();
        private readonly List<Guid> _repliedComments = new List<Guid>();
    }
}