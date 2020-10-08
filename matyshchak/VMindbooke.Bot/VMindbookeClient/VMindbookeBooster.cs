using System;
using System.Collections;
using System.Collections.Generic;
using VMindbookeClient.Domain;

namespace VMindbookeClient
{
    public class VMindbookeBooster
    {
        private IVMindbookeClient _client;

        public VMindbookeBooster(IVMindbookeClient client)
        {
            _client = client;
        }

        public void CommentPostsWithLikes(int userToBoostId,
            string userToBoostToken,
            int minNumberOfLikesToCommentPost,
            CommentContent comment)
        {
            Console.WriteLine($"Added comment to post with id ");
            var posts = _client.GetAllPosts();
            foreach (var post in posts)
            {
                if (post.Likes.Count >= minNumberOfLikesToCommentPost)
                {
                    _client.CommentPost(userToBoostId, userToBoostToken, post.Id, comment);
                }
            }
        }
    }
}