using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VMindbooke.SDK;
using VMindbookeClient.Domain;

namespace VMindbookeClient
{
    public class VmBooster
    {
        private readonly IVmClient _client;
        private HashSet<int> _commentedPostsIds = new HashSet<int>();
        private HashSet<Guid> _repliedCommentsIds = new HashSet<Guid>();
        private HashSet<int> _stolenPostsIds = new HashSet<int>();

        public VmBooster(IVmClient client)
        {
            _client = client;
        }

        public void CommentPostsWithLikes(int userToBoostId,
            string userToBoostToken,
            int minNumberOfDailyLikesToCommentPost,
            CommentContent comment)
        {
            var posts = _client.GetAllPosts();
            foreach (var post in posts)
            {
                var numberOfDailyLikes = post
                    .Likes
                    .Count(like => like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day);
                    
                if (numberOfDailyLikes < minNumberOfDailyLikesToCommentPost)
                    continue;
                if (_commentedPostsIds.Contains(post.Id))
                {
                    Console.WriteLine($"post {post.Id} is already commented");
                    continue;
                }

                Console.WriteLine($"Added comment to post with id ");
                _client.CommentPost(userToBoostId, userToBoostToken, post.Id, comment);
                _commentedPostsIds.Add(post.Id);
            }
        }
        
        public void ReplyToCommentWithLikes(int userToBoostId,
            string userToBoostToken,
            int minNumberOfLikesToReplyToComment,
            CommentContent reply)
        {
            var comments = _client.GetAllComments();
            var posts = _client.GetAllPosts();
            foreach (var post in posts)
            {
                foreach (var comment in post.Comments)
                {
                    var numberOfDailyLikes = comment.Likes.Count(like =>
                        like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day);

                    if (numberOfDailyLikes < minNumberOfLikesToReplyToComment)
                        continue;
                    if (_repliedCommentsIds.Contains(comment.Id))
                    {
                        Console.WriteLine($"comment {comment.Id} is already replied");
                        continue;
                    }

                    Console.WriteLine($"Added reply to post with id {comment.Id}");
                    _client.ReplyToComment(userToBoostToken, post.Id, comment.Id, reply);
                    _repliedCommentsIds.Add(comment.Id);
                }
            }
        }
        
        public void StealPostWithLikes(int userToBoostId,
            string userToBoostToken,
            int minNumberOfDailyLikesToStealPost,
            string newPostTitle)
        {
            var posts = _client.GetAllPosts();
            foreach (var post in posts)
            {
                var numberOfDailyLikes = post
                    .Likes
                    .Count(like => like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day);
                    
                if (numberOfDailyLikes < minNumberOfDailyLikesToStealPost)
                    continue;
                if (_stolenPostsIds.Contains(post.Id))
                {
                    Console.WriteLine($"post {post.Id} is already stolen");
                    continue;
                }

                Console.WriteLine($"Stole post with id {post.Id}");
                _client.Post(userToBoostId, userToBoostToken, new PostContent(newPostTitle, post.Content));
                _commentedPostsIds.Add(post.Id);
            }
        }
        
        public void StealTheBestPostOfMostLikedUser(int userToBoostId,
            string userToBoostToken,
            int minDailyUserLikesToStealHisBestPost,
            string newPostTitle)
        {
            var users = _client.GetAllUsers();

            foreach (var user in users)
            {
                var numberOfDailyLikes = user
                    .Likes
                    .Count(like => like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day);
                
                if (numberOfDailyLikes < minDailyUserLikesToStealHisBestPost)
                    continue;

                var postToSteal = _client
                    .GetUserPosts(user.Id)
                    .OrderByDescending(post => post.Likes.Count)
                    .First();
                
                if (_stolenPostsIds.Contains(postToSteal.Id))
                {
                    Console.WriteLine($"post {postToSteal.Id} is already stolen");
                    continue;
                }

                Console.WriteLine($"Stole best post of user {user.Name} with id {postToSteal.Id}");
                _client.Post(userToBoostId, userToBoostToken, new PostContent(postToSteal.Title, postToSteal.Content));
                _commentedPostsIds.Add(postToSteal.Id);
            }
        }
    }
}