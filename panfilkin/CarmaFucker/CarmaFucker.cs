using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Serilog;
using VMindBooke.SDK.Application;
using VMindBooke.SDK.Domain;

namespace CarmaFucker
{
    public class CarmaFucker
    {
        private IVMindBookeClient _vMindBookeClient;
        private User _mainUser;
        private int _postLikesToComment;
        private int _commentLikesToReply;
        private int _postLikesToCopyPost;
        private int _userLikesToCopyPost;
        private int _selfLikesToStop;

        public CarmaFucker(IVMindBookeClient vMindBookeClient, User mainUser, int postLikesToComment,
            int commentLikesToReply, int postLikesToCopyPost, int userLikesToCopyPost, int selfLikesToStop)
        {
            _vMindBookeClient = vMindBookeClient ?? throw new ArgumentNullException(nameof(vMindBookeClient));
            _mainUser = mainUser ?? throw new ArgumentNullException(nameof(mainUser));
            _postLikesToComment = postLikesToComment;
            _commentLikesToReply = commentLikesToReply;
            _postLikesToCopyPost = postLikesToCopyPost;
            _userLikesToCopyPost = userLikesToCopyPost;
            _selfLikesToStop = selfLikesToStop;
        }

        public void CommentLikedPosts(int postLikesToComment)
        {
            Log.Information("Searching most liked posts to comment...");
            var postsToLike = _vMindBookeClient.PostService.GetAllPosts()
                .Where(post => post.Likes.Count() > postLikesToComment)
                .Where(post => !post.Likes.Select(like => like.AuthorId).Contains(_mainUser.Id));
            foreach (var post in postsToLike)
            {
                _vMindBookeClient.PostService.CommentPost(post, _mainUser, ContentGenerator.GetContent());
            }
            Log.Information($"Commented {postsToLike.Count()} posts!");
        }
        
        public void ReplyLikedComments(int commentLikesToReply)
        {
            Log.Information("Searching most liked comments to reply...");
            int successfulReplies = 0;
            var postsWithCommentsToReply = _vMindBookeClient.PostService.GetAllPosts()
                .Where(post => post.Comments.Count(comment => comment.Likes.Count() > commentLikesToReply) > 0);
            foreach (var post in postsWithCommentsToReply)
            {
                foreach (var comment in post.Comments)
                {
                    if (comment.Likes.Count() > commentLikesToReply && !comment.Likes.Select(like => like.AuthorId).Contains(_mainUser.Id))
                    {
                        _vMindBookeClient.PostService.ReplyComment(post, comment, _mainUser, ContentGenerator.GetContent());
                        successfulReplies++;
                    }
                }
            }
            Log.Information($"Replied to {successfulReplies} comments!");
        }
        
        public void CopyPostLikedPosts(int postLikesToCopyPost)
        {
            Log.Information("Searching most liked posts to comment...");
            var postsToCopyPost = _vMindBookeClient.PostService.GetAllPosts()
                .Where(post => post.Likes.Count() > postLikesToCopyPost);
            foreach (var post in postsToCopyPost)
            {
                _vMindBookeClient.UserService.CreatePost(_mainUser, ContentGenerator.GetTitle(), post.Content);
            }
            Log.Information($"Copied {postsToCopyPost.Count()} most liked posts!");
        }

        public void CopyPostLikedUserPost(int userLikesToCopyPost)
        {
            Log.Information("Searching most liked users to copy post...");
            int successfulCopyPosts = 0;
            var usersWithBestPosts = _vMindBookeClient.UserService.GetAllUsers().Where(user => user.LikesList.Count() > userLikesToCopyPost);
            foreach (var user in usersWithBestPosts)
            {
                var bestUserPost = _vMindBookeClient.UserService.GetUserPosts(user)
                    .OrderByDescending(post => post.Likes.Count()).First();
                if (bestUserPost != null)
                {
                    _vMindBookeClient.UserService.CreatePost(_mainUser, ContentGenerator.GetTitle(), bestUserPost.Content);
                    successfulCopyPosts++;
                }
            }
            Log.Information($"Copied {successfulCopyPosts} posts from most liked users!");
        }

        public bool IsContinueCheating(int selfLikesToStop)
        {
            Log.Information("Checking user likes...");
            var user = _vMindBookeClient.UserService.UserLikesUpdate(_mainUser);
            var likesCountLastDay = user.LikesList.Count(like => (DateTime.Now - like.PlacingDateUtc).Days == 0);
            Log.Information($"You have {likesCountLastDay} new likes since last day!");
            return likesCountLastDay < selfLikesToStop;
        }
    }
}