using System;
using System.Collections.Generic;
using System.Linq;
using LikesCheating.Infrastructure;
using VMindbooke.SDK;

namespace LikesCheating.Domain
{
    public class Cheating : ICheating
    {
        public Cheating()
        {
            _logger = new Logger("actions.log");
            var configuration = new Configuration();
            _client = new VMindbookeClient(configuration.GetValue("VMindbookeUrl"));
            
            _thresholds = new Thresholds(
                Convert.ToInt32(configuration.GetValue("PostThresholdToComment")),
                Convert.ToInt32(configuration.GetValue("PostThresholdToDuplicate")),
                Convert.ToInt32(configuration.GetValue("CommentThresholdToReply")),
                Convert.ToInt32(configuration.GetValue("UserSuccessPostThreshold")),
            Convert.ToInt32(configuration.GetValue("UserTargetThreshold"))
            );
            
            _commentedPosts = ActionsRepository.Load<List<int>>(_commentedPostsPath);
            _duplicatedPosts = ActionsRepository.Load<List<int>>(_duplicatedPostsPath);;
            _repliedComments = ActionsRepository.Load<List<Guid>>(_repliedCommentsPath);;
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
                    ActionsRepository.Save(_commentedPostsPath, _commentedPosts);
                    _logger.Information($"Comment to post {post.Id} added");
                }
            }
        }

        public void ReplyIfLikesMoreThanThreshold(string token)
        {
            var posts = _client.GetPosts();
            var postComments = posts.SelectMany(p => p.Comments,
                (p, c) => new {Post = p, Comment = c});
            foreach (var postComment in postComments)
            {
                if (postComment.Comment.Likes.Where(like => like.PlacingDateUtc < DateTime.Today).Count() >
                    _thresholds.CommentThresholdToReply
                    && !_repliedComments.Contains(postComment.Comment.Id))
                {
                    var replyContent = new ReplyContent("Hello there! I'm using likes cheating");
                    _client.Reply(replyContent, postComment.Post.Id, postComment.Comment.Id, token);
                    _repliedComments.Add(postComment.Comment.Id);
                    ActionsRepository.Save(_repliedCommentsPath, _repliedComments);
                    _logger.Information($"Reply to comment {postComment.Comment.Id} added");
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
                    ActionsRepository.Save(_duplicatedPostsPath, _duplicatedPosts);
                    _logger.Information($"Post {post.Id} duplicated");
                }
            }
        }

        public void DuplicateMostSuccessfulPostIfLikesMoreThanThreshold(int userId, string token)
        {
            var users = _client.GetUsers();
            var posts = users.SelectMany(u => _client.GetUserPosts(u.Id),
                (u, p) => new {User = u, Post = p}).Select(up => up.Post);
            foreach (var post in posts)
            {
                if (post.Likes.Where(like => like.PlacingDateUtc < DateTime.Today).Count() >
                    _thresholds.UserSuccessPostThreshold
                    && !_duplicatedPosts.Contains(post.Id))
                {
                    var postContent = new PostContent(post.Title, post.Content);
                    _client.Post(postContent, userId, token);
                    _duplicatedPosts.Add(post.Id);
                    ActionsRepository.Save(_duplicatedPostsPath, _duplicatedPosts);
                    _logger.Information($"Post {post.Id} duplicated");
                }
            }
        }

        public bool CheckUserReachedLikesThreshold(int userId)
        {
            var user = _client.GetUser(userId);
            _logger.Information($"User {userId} has {user.Likes.Count} likes");
            return user.Likes.Count < _thresholds.UserDailyTargetThreshold;
        }

        private readonly ILogger _logger;
        private readonly IVMindbookeClient _client;
        private readonly Thresholds _thresholds;
        private List<int> _commentedPosts;
        private List<int> _duplicatedPosts;
        private List<Guid> _repliedComments;
        private const string _commentedPostsPath = "commentedPosts.json";
        private const string _duplicatedPostsPath = "duplicatedPosts.json";
        private const string _repliedCommentsPath = "repliedComments.json";
    }
}