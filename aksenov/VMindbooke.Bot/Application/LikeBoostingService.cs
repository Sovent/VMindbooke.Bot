using System;
using System.Collections.Generic;
using System.Linq;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Application
{
    public class LikeBoostingService
    {
        private readonly BotSettings _settings;
        private readonly APIRequestsService _apiService;
        private DateTime _currentDate;
        private bool _isRunning;
        
        private readonly ISpamRepository _spamRepository;
        private readonly IHashesRepository _hashesRepository;
        private readonly IProcessedObjectsRepository _processedObjectsRepository;

        public LikeBoostingService(BotSettings settings, APIRequestsService apiService, DateTime currentDate, ISpamRepository spamRepository, IHashesRepository hashesRepository, IProcessedObjectsRepository processedObjectsRepository)
        {
            _settings = settings;
            _apiService = apiService;
            _currentDate = currentDate;
            _spamRepository = spamRepository;
            _hashesRepository = hashesRepository;
            _processedObjectsRepository = processedObjectsRepository;
            _isRunning = true;
        }

        public void CommentsWritingScenario()
        {
            if (!_isRunning) return;
            
            var unprocessedPosts = GetUnprocessedPosts();

            foreach (var post in unprocessedPosts)
            {
                if (post.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                    _settings.LikeLimitForPostToMakeComment)
                {
                    if (!_processedObjectsRepository.DoesContainCommentedPostWith(post.Id))
                    {
                        _apiService.PostComment(post.Id, _spamRepository.GetRandomComment());
                        _processedObjectsRepository.AddNewCommentedPost(post.Id);
                    }
                }
            }
        }

        public void RepliesWritingScenario()
        {
            if (!_isRunning) return;
            
            var unprocessedPosts = GetUnprocessedPosts();

            foreach (var post in unprocessedPosts)
            {
                foreach (var comment in post.Comments)
                {
                    if (comment.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                        _settings.LikeLimitForCommentToMakeReply)
                    {
                        if (!_processedObjectsRepository.DoesContainRepliedCommentWith(comment.Id))
                        {
                            _apiService.ReplyToComment(post.Id, comment.Id, _spamRepository.GetRandomReply());
                            _processedObjectsRepository.AddNewRepliedComment(comment.Id);
                        }
                    }
                }
            }
        }

        public void PostsCopyingByLikesScenario()
        {
            if (!_isRunning) return;
            
            var unprocessedPosts = GetUnprocessedPosts();

            foreach (var post in unprocessedPosts)
            {
                if (post.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                    _settings.LikeLimitForPostToCopy)
                {
                    if (!_processedObjectsRepository.DoesContainCopiedPostWith(post.Id))
                    {
                        _apiService.CreatePost(_spamRepository.GetRandomPostTitle(), post.Content);
                        _processedObjectsRepository.AddNewCopiedPost(post.Id);
                    }
                }
            }
        }

        public void PostsCopyingByUsersScenario()
        {
            if (!_isRunning) return;
            
            var unprocessedUsers = GetUnprocessedUsers();

            foreach (var user in unprocessedUsers)
            {
                if (user.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                    _settings.LikeLimitForUserToCopyPost)
                {
                    CopyTheMostLikedPost(user.Id);
                }
            }
        }

        public void BoostFinishScenario()
        {
            if (!_isRunning)
            {
                if (_currentDate.Date != DateTime.Now.Date)
                {
                    _currentDate = DateTime.Now;
                    _isRunning = true;
                }
                return;
            }
            
            var user = _apiService.GetUser(_settings.UserId);

            if (user.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                _settings.LikeLimitToCompleteProcess)
            {
                _isRunning = false;
                throw new NotImplementedException("logger");
            }
        }

        private IEnumerable<Post> GetUnprocessedPosts()
        {
            var posts = _apiService.GetPosts();

            var unprocessedPosts = new List<Post>();

            foreach (var post in posts)
            {
                if (_hashesRepository.DoesContainPostHashWith(post.Id))
                {
                    if (_hashesRepository.GetPostHashBy(post.Id) != post.GetHashCode())
                    {
                        _hashesRepository.AddOrUpdatePostHash(post.Id, post.GetHashCode());
                        unprocessedPosts.Add(post);
                    }
                }
                else
                {
                    _hashesRepository.AddOrUpdatePostHash(post.Id, post.GetHashCode());
                    unprocessedPosts.Add(post);
                }
            }

            return unprocessedPosts;
        }

        private IEnumerable<User> GetUnprocessedUsers()
        {
            var users = _apiService.GetUsers();

            var unprocessedUsers = new List<User>();

            foreach (var user in users)
            {
                if (_hashesRepository.DoesContainUserHashWith(user.Id))
                {
                    if (_hashesRepository.GetUserHashBy(user.Id) != user.GetHashCode())
                    {
                        _hashesRepository.AddOrUpdateUserHash(user.Id, user.GetHashCode());
                        unprocessedUsers.Add(user);
                    }
                }
                else
                {
                    _hashesRepository.AddOrUpdateUserHash(user.Id, user.GetHashCode());
                    unprocessedUsers.Add(user);
                }
            }

            return unprocessedUsers;
        }

        private void CopyTheMostLikedPost(int userId)
        {
            var theMostLikedUserPost = GetTheMostLikedUserPost(userId);
            if (!_processedObjectsRepository.DoesContainCopiedPostWith(theMostLikedUserPost.Id))
            {
                _apiService.CreatePost(theMostLikedUserPost.Title, theMostLikedUserPost.Content);
                _processedObjectsRepository.AddNewCopiedPost(theMostLikedUserPost.Id);
            }
        }

        private Post GetTheMostLikedUserPost(int userId)
        {
            var posts = _apiService.GetUserPosts(userId);
            return posts
                .OrderByDescending(post => post.Likes.Length)
                .FirstOrDefault();
        }
    }
}