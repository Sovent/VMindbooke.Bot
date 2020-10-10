using System;
using System.Collections.Generic;
using System.Linq;
using VMindbookeBooster.Entities;

namespace VMindbookeBooster
{
    public class PostCommenter : IPostCommenter
    {
        private readonly UserCredentials _userCredentials;
        private readonly IVmClient _client;
        private readonly HashSet<int> _commentedPostsIds = new HashSet<int>();

        public PostCommenter(UserCredentials userCredentials, IVmClient client)
        {
            _client = client;
            _userCredentials = userCredentials;
        }

        public void CommentPosts(int likesThreshold, CommentContent comment)
        {
            Console.WriteLine("TYING TO COMMENT");
            var posts = _client.GetAllPosts();
            foreach (var post in posts)
            {
                var numberOfDailyLikes = post
                    .Likes
                    .Count(like => like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day);
                    
                if (numberOfDailyLikes < likesThreshold)
                    continue;
                if (_commentedPostsIds.Contains(post.Id))
                {
                    Console.WriteLine($"post {post.Id} is already commented");
                    continue;
                }

                Console.WriteLine($"Added comment to post with id: {post.Id} title: {post.Title}");
                _client.CommentPost(_userCredentials.Id, _userCredentials.Token, post.Id, comment);
                _commentedPostsIds.Add(post.Id);
            }
        }
    }
}