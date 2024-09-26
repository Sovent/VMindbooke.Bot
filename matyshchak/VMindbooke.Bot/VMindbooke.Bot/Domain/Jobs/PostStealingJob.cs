﻿using System;
using System.Collections.Generic;
using System.Linq;
using Usage.Domain.ContentProviders;
using Usage.Domain.ValueObjects;
using Usage.Domain.ValueObjects.LikeThresholds;
using Usage.Infrastructure;

namespace Usage.Domain.Jobs
{
    public class PostStealingJob : IBoostingJob
    {
        public PostStealingJob(UserCredentials userCredentials,
            IVmClient client,
            PostLikesToStealThreshold postLikesThreshold,
            UserLikesToStealPostThreshold userLikesThreshold, IPostTitleProvider postTitleProvider)
        {
            _userCredentials = userCredentials;
            _client = client;
            _postLikesThreshold = postLikesThreshold;
            _userLikesThreshold = userLikesThreshold;
            _postTitleProvider = postTitleProvider;
        }
        
        private readonly UserCredentials _userCredentials;
        private readonly IVmClient _client;
        private readonly PostLikesToStealThreshold _postLikesThreshold;
        private readonly UserLikesToStealPostThreshold _userLikesThreshold;
        private readonly IPostTitleProvider _postTitleProvider;
        private readonly HashSet<int> _stolenPostsIds = new HashSet<int>();
        
        public void Execute()
        {
            StealPostsContent();
            StealMostLikedPostsFromUsers();
        }

        private void StealPostsContent()
        {
            var selectedPosts = _client.GetAllPosts()
                .Where(post =>
                    post.Likes.Count(like => like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day)
                    < _postLikesThreshold.Value &&
                    !_stolenPostsIds.Contains(post.Id));
            
            foreach (var post in selectedPosts)
            {
                _client.Post(_userCredentials.Id,
                    _userCredentials.Token,
                    new PostRequest(_postTitleProvider.GetPostTitle(),
                        post.Content));
                _stolenPostsIds.Add(post.Id);
            }
        }
        
        private void StealMostLikedPostsFromUsers()
        {
            var users = _client.GetAllUsers();

            foreach (var user in users)
            {
                
                var numberOfDailyLikes = user
                    .Likes
                    .Count(like => like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day);
                
                if (numberOfDailyLikes < _userLikesThreshold.Value)
                    continue;

                var postToSteal = _client
                    .GetUserPosts(user.Id)
                    .OrderByDescending(post => post.Likes.Count)
                    .First();
                
                if (_stolenPostsIds.Contains(postToSteal.Id))
                    continue;

                _client.Post(_userCredentials.Id, _userCredentials.Token, new PostRequest(postToSteal.Title, postToSteal.Content));
                _stolenPostsIds.Add(postToSteal.Id);
            }
        }
    }
}