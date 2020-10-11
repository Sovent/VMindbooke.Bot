using System;
using System.Collections.Generic;
using System.Linq;
using Usage.Domain.ContentProviders;
using Usage.Domain.Entities;
using Usage.Domain.ValueObjects;

namespace Usage.Domain.Jobs
{
    public class PostCommentingJob : IBoostingJob
    {
        public PostCommentingJob(UserCredentials userCredentials, IVmClient client, ICommentContentProvider commentContentProvider, PostLikesToCommentThreshold likesThreshold)
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
            Console.WriteLine("TYING TO COMMENT");
            var posts = _client.GetAllPosts();
            foreach (var post in posts)
            {
                var numberOfDailyLikes = post
                    .Likes
                    .Count(like => like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day);
                    
                if (numberOfDailyLikes < _likesThreshold.Value)
                    continue;
                if (_commentedPostsIds.Contains(post.Id))
                {
                    Console.WriteLine($"post {post.Id} is already commented");
                    continue;
                }

                Console.WriteLine($"Added comment to post with id: {post.Id} title: {post.Title}");
                _client.CommentPost(_userCredentials.Id, _userCredentials.Token, post.Id, _commentContentProvider.GetCommentContent());
                _commentedPostsIds.Add(post.Id);
            }
        }
    }
}