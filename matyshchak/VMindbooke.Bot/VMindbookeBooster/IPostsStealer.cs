using System;
using System.Collections.Generic;
using System.Linq;
using VMindbookeBooster.Entities;

namespace VMindbookeBooster
{
    public interface IPostsStealer
    {
        void StealPosts(int likesThreshold, string newPostTitle);

        void StealTheBestPostOfMostLikedUser(int userLikesThreshold);
    }

    public class PostsStealer : IPostsStealer
    {
        public PostsStealer(UserCredentials userCredentials, IVmClient client)
        {
            _userCredentials = userCredentials;
            _client = client;
        }
        
        private readonly UserCredentials _userCredentials;
        private readonly IVmClient _client;
        private readonly HashSet<int> _stolenPostsIds = new HashSet<int>();
        
        public void StealPosts(int likesThreshold, string newPostTitle)
        {
            var posts = _client.GetAllPosts();
            foreach (var post in posts)
            {
                var numberOfDailyLikes = post
                    .Likes
                    .Count(like => like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day);
                    
                if (numberOfDailyLikes < likesThreshold)
                    continue;
                
                if (_stolenPostsIds.Contains(post.Id))
                {
                    Console.WriteLine($"post {post.Id} is already stolen");
                    continue;
                }

                Console.WriteLine($"Stole post with id {post.Id}");
                _client.Post(_userCredentials.Id, _userCredentials.Token, new PostContent(newPostTitle, post.Content));
                _stolenPostsIds.Add(post.Id);
            }
        }
        
        public void StealTheBestPostOfMostLikedUser(
            int userLikesThreshold)
        {
            var users = _client.GetAllUsers();

            foreach (var user in users)
            {
                
                var numberOfDailyLikes = user
                    .Likes
                    .Count(like => like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day);
                
                if (numberOfDailyLikes < userLikesThreshold)
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
                _client.Post(_userCredentials.Id, _userCredentials.Token, new PostContent(postToSteal.Title, postToSteal.Content));
                _stolenPostsIds.Add(postToSteal.Id);
            }
        }
    }
}