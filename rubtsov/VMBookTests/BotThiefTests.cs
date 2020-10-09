using System;
using System.Collections.Generic;
using VMBookeBot.Domain;
using VMBookeBot.Infrastructure;
using NUnit.Framework;
using VMBook.SDK;

namespace VMBookTests
{
    public class Tests
    {
        private BotThief _botThief;
        private List<Post> _posts;
        private List<User> _users;
        private List<int> _alreadyStolenPosts;
        private List<int> _alreadyCommentedPosts;
        private List<Guid> _alreadyRepliedComments;
        private RepositoryForTests _repository;
        
        
        public Tests()
        {
            _alreadyStolenPosts = new List<int>();
            _alreadyCommentedPosts = new List<int>();
            _alreadyRepliedComments = new List<Guid>();
            var userActivityRepository = new UserActivityRepository(_alreadyStolenPosts,
                _alreadyRepliedComments,
                _alreadyCommentedPosts);
            _posts = new List<Post>();
            _users = new List<User>();
            _repository = new RepositoryForTests(_posts, _users);
            var requestRetryer = new TestsRequestRetryer(_repository);
            var requestFilter = new TestsRequestFilter(_repository);
            _botThief = new BotThief(requestRetryer, requestFilter, userActivityRepository);
        }
        [SetUp]
        public void Setup()
        {
            _posts.Clear();
            _users.Clear();
            _repository.Clear();
            _alreadyStolenPosts.Clear();
            _alreadyCommentedPosts.Clear();
            _alreadyRepliedComments.Clear();
        }
        
        [Test]
        public void WriteCommentUnderAlreadyCommentedPost_ThePostSkipped()
        {
            var commentedPost = new Post(1, 1, new List<Comment>());
            _posts.Add(commentedPost);
            _alreadyCommentedPosts.Add(_posts[0].Id);
            const int expected = 0;
            
            _botThief.WriteCommentUnderPopularPosts(_posts);
            var actual = commentedPost.Comments.Count;
            
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void WriteCommentUnderPopularPosts_ThePostCommentsNumberIncreased()
        {
            var newPostToComment = new Post(1, 1, new List<Comment>());
            _posts.Add(newPostToComment);
            const int expected = 1;
            
            _botThief.WriteCommentUnderPopularPosts(_posts);
            var actual = newPostToComment.Comments.Count;
            
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, _alreadyCommentedPosts.Count);
        }
    }
}