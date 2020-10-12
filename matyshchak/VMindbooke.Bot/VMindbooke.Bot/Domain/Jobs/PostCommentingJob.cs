using System;
using System.Collections.Generic;
using System.Linq;
using Usage.Domain.ContentProviders;
using Usage.Domain.ValueObjects;
using Usage.Domain.ValueObjects.LikeThresholds;

namespace Usage.Domain.Jobs
{
    public class PostCommentingJob : IBoostingJob
    {
        public PostCommentingJob(UserCredentials userCredentials,
            IVmClient client,
            ICommentContentProvider commentContentProvider,
            PostLikesToCommentThreshold likesThreshold)
        {
            _userCredentials = userCredentials;
            _client = client;
            _commentContentProvider = commentContentProvider;
            _likesThreshold = likesThreshold;
        }
        
        private readonly UserCredentials _userCredentials;
        private readonly IVmClient _client;
        private readonly ICommentContentProvider _commentContentProvider;
        private readonly PostLikesToCommentThreshold _likesThreshold;
        private readonly HashSet<int> _commentedPostsIds = new HashSet<int>();

        public void Execute()
        {
            var selectedPosts = _client.GetAllPosts()
                .Where(post =>
                    post.Likes.Count(like => like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day)
                    < _likesThreshold.Value &&
                    !_commentedPostsIds.Contains(post.Id));

            foreach (var post in selectedPosts)
            {
                _client.CommentPost(_userCredentials.Id, _userCredentials.Token, post.Id, _commentContentProvider.GetCommentContent());
                _commentedPostsIds.Add(post.Id);
            }
        }
    }
}