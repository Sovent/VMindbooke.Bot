using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using VMindBooke.SDK.Application;
using VMindBooke.SDK.Domain;

namespace CarmaFucker
{
    public class CarmaFucker
    {
        private readonly int _commentLikesToReply;
        private readonly User _mainUser;

        private readonly int _postLikesToComment;
        private readonly int _postLikesToCopyPost;
        private readonly int _selfLikesToStop;
        private readonly int _userLikesToCopyPost;
        private readonly VMindBookeClient _vMindBookeClient;

        public CarmaFucker()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var serverAddress = configuration.GetValue<string>("serverAddress");
            var userId = configuration.GetValue<int>("userId");
            var userToken = configuration.GetValue<string>("userToken");
            _postLikesToComment = configuration.GetValue<int>("postLikesToComment");
            _commentLikesToReply = configuration.GetValue<int>("commentLikesToReply");
            _postLikesToCopyPost = configuration.GetValue<int>("postLikesToCopyPost");
            _userLikesToCopyPost = configuration.GetValue<int>("userLikesToCopyPost");
            _selfLikesToStop = configuration.GetValue<int>("selfLikesToStop");

            _vMindBookeClient = VMindBookeFactory.VMindBookeClientBuild(serverAddress);
            _mainUser = _vMindBookeClient.UserService.GetAuthorizedUser(userId, userToken);
        }

        public CarmaFucker(VMindBookeClient client)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var userId = configuration.GetValue<int>("userId");
            var userToken = configuration.GetValue<string>("userToken");
            _postLikesToComment = configuration.GetValue<int>("postLikesToComment");
            _commentLikesToReply = configuration.GetValue<int>("commentLikesToReply");
            _postLikesToCopyPost = configuration.GetValue<int>("postLikesToCopyPost");
            _userLikesToCopyPost = configuration.GetValue<int>("userLikesToCopyPost");
            _selfLikesToStop = configuration.GetValue<int>("selfLikesToStop");

            _vMindBookeClient = client;
            _mainUser = _vMindBookeClient.UserService.GetAuthorizedUser(userId, userToken);
        }

        private static List<int> LoadCopiedPostIdFromFile()
        {
            var data = JsonConvert.DeserializeObject<List<int>>(File.ReadAllText("copiedPostId"));
            return data != null ? data.Distinct().ToList() : new List<int>();
        }

        private static void SaveCopiedPostIdToFile(List<int> copiedPostIdList)
        {
            File.WriteAllText("copiedPostId", JsonConvert.SerializeObject(copiedPostIdList));
        }

        [MaximumConcurrentExecutions(1, timeoutInSeconds: 3600, pollingIntervalInSeconds: 120)]
        public void CommentLikedPosts()
        {
            Log.Information("[COMM] Looking for the most liked posts of the last day to comment...");
            var postsToLike = new List<Post>(_vMindBookeClient.PostService.GetAllPosts()
                .Where(post => (DateTime.Now - post.PostingDateUtc).Days == 0)
                .Where(post => post.Likes.Count > _postLikesToComment)
                .Where(post => !post.Comments.Select(comment => comment.AuthorId).Contains(_mainUser.Id)));

            Log.Information($"[COMM] Found {postsToLike.Count} posts to comment! Commenting...");
            foreach (var post in postsToLike)
            {
                _vMindBookeClient.PostService.CommentPost(post, _mainUser, ContentGenerator.GetContent());
            }

            Log.Information("[COMM] Complete!");
        }

        [MaximumConcurrentExecutions(1, timeoutInSeconds: 3600, pollingIntervalInSeconds: 120)]
        public void ReplyLikedComments()
        {
            Log.Information("[REPL] Looking for the most liked comments of the last day to reply...");

            var postsWithCommentsToReply = _vMindBookeClient.PostService.GetAllPosts()
                .Select(post => new KeyValuePair<Post, List<Comment>>(
                    post,
                    new List<Comment>(post.Comments
                        .Where(comment => (DateTime.Now - comment.PostingDateUtc).Days == 0)
                        .Where(comment => comment.Likes.Count > _commentLikesToReply)
                        .Where(comment => !comment.Replies.Select(reply => reply.AuthorId).Contains(_mainUser.Id))
                    ))
                )
                .Where(post => post.Value.Any());
            var withCommentsToReply = postsWithCommentsToReply.ToList();
            int replyCount = withCommentsToReply.SelectMany(post => post.Value).Count();
            Log.Information(
                $"[REPL] Found {withCommentsToReply.Count} posts with {replyCount} comments that you can reply to! Replying...");
            foreach (var postPair in withCommentsToReply)
            {
                foreach (var comment in postPair.Value)
                {
                    _vMindBookeClient.PostService.ReplyComment(postPair.Key, comment, _mainUser,
                        ContentGenerator.GetContent());
                }
            }

            Log.Information("[REPL] Complete!");
        }

        [MaximumConcurrentExecutions(1, timeoutInSeconds: 3600, pollingIntervalInSeconds: 120)]
        public void CopyPostLikedPosts()
        {
            Log.Information("[BPST] Looking for the most liked posts of the last day to copy post...");
            var copiedPostsIdList = LoadCopiedPostIdFromFile();
            var newCopiedPostsIdList = new List<int>();

            var postsToCopyPost = _vMindBookeClient.PostService.GetAllPosts()
                .Where(post => (DateTime.Now - post.PostingDateUtc).Days == 0)
                .Where(post => post.Likes.Count > _postLikesToCopyPost)
                .Where(post => !copiedPostsIdList.Contains(post.Id));
            var toCopyPost = postsToCopyPost.ToList();
            Log.Information($"[BPST] Found {toCopyPost.Count} posts to copy! Copying...");
            foreach (var post in toCopyPost)
            {
                _vMindBookeClient.UserService.CreatePost(_mainUser, ContentGenerator.GetTitle(), post.Content);
                newCopiedPostsIdList.Add(post.Id);
            }

            Log.Information("[BPST] Complete!");

            copiedPostsIdList = LoadCopiedPostIdFromFile();
            copiedPostsIdList.AddRange(newCopiedPostsIdList);
            SaveCopiedPostIdToFile(copiedPostsIdList);
        }

        [MaximumConcurrentExecutions(1, timeoutInSeconds: 3600, pollingIntervalInSeconds: 120)]
        public void CopyPostLikedUserPost()
        {
            Log.Information("[UPST] Looking for the most liked users of the last day to copy their best posts...");
            var copiedPostsIdList = LoadCopiedPostIdFromFile();
            var newCopiedPostsIdList = new List<int>();

            var bestUsersPosts = _vMindBookeClient.UserService.GetAllUsers()
                .Where(user => user.Likes.Count(like => (DateTime.Now - like.PlacingDateUtc).Days == 0) >
                               _userLikesToCopyPost)
                .Where(user => user.Id != _mainUser.Id)
                .Select(user => _vMindBookeClient.UserService
                    .GetUserPosts(user)
                    .Where(post => !copiedPostsIdList.Contains(post.Id))
                    .OrderByDescending(post => post.Likes.Count)
                    .FirstOrDefault())
                .Where(post => post != null)
                .ToList();
            Log.Information($"[UPST] Found {bestUsersPosts.Count} posts of best users to copy! Copying...");
            foreach (var post in bestUsersPosts)
            {
                if (post != null)
                {
                    _vMindBookeClient.UserService.CreatePost(_mainUser, ContentGenerator.GetTitle(), post.Content);
                    newCopiedPostsIdList.Add(post.Id);
                }
            }

            Log.Information("[UPST] Complete!");

            copiedPostsIdList = LoadCopiedPostIdFromFile();
            copiedPostsIdList.AddRange(newCopiedPostsIdList);
            SaveCopiedPostIdToFile(copiedPostsIdList);
        }

        [MaximumConcurrentExecutions(1, timeoutInSeconds: 3600, pollingIntervalInSeconds: 120)]
        public bool IsContinueCheating()
        {
            var user = _vMindBookeClient.UserService.UserLikesUpdate(_mainUser);
            var likesCountLastDay = user.Likes.Count(like => (DateTime.Now - like.PlacingDateUtc).Days == 0);
            Log.Information($"You have {likesCountLastDay} likes last day!");
            return likesCountLastDay < _selfLikesToStop;
        }
    }
}