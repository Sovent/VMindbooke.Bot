using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Core;
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

        private readonly Logger _logger;

        public LikeBoostingService(BotSettings settings, APIRequestsService apiService, DateTime currentDate, ISpamRepository spamRepository, IHashesRepository hashesRepository, IProcessedObjectsRepository processedObjectsRepository, Logger logger)
        {
            _settings = settings;
            _apiService = apiService;
            _currentDate = currentDate;
            _spamRepository = spamRepository;
            _hashesRepository = hashesRepository;
            _processedObjectsRepository = processedObjectsRepository;
            _logger = logger;
            _isRunning = true;
        }

        public void CommentsWritingScenario()
        {
            if (!_isRunning) return;
            
            var unprocessedPosts = GetUnprocessedPosts();
            if (unprocessedPosts == null) return;

            foreach (var post in unprocessedPosts)
            {
                if (post.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                    _settings.LikeLimitForPostToMakeComment)
                {
                    if (!_processedObjectsRepository.DoesContainCommentedPostWith(post.Id))
                    {
                        var isSuccessful = _apiService.PostComment(post.Id, _spamRepository.GetRandomComment());
                        if (isSuccessful)
                        {
                            _processedObjectsRepository.AddNewCommentedPost(post.Id);
                            _logger.Information($"Comment to post [{post.Id}] added.");
                        }
                    }
                }
            }
        }

        public void RepliesWritingScenario()
        {
            if (!_isRunning) return;
            
            var unprocessedPosts = GetUnprocessedPosts();
            if (unprocessedPosts == null) return;

            foreach (var post in unprocessedPosts)
            {
                foreach (var comment in post.Comments)
                {
                    if (comment.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                        _settings.LikeLimitForCommentToMakeReply)
                    {
                        if (!_processedObjectsRepository.DoesContainRepliedCommentWith(comment.Id))
                        {
                            var isSuccessful = _apiService.ReplyToComment(post.Id, comment.Id, _spamRepository.GetRandomReply());
                            if (isSuccessful)
                            {
                                _processedObjectsRepository.AddNewRepliedComment(comment.Id);
                                _logger.Information($"Reply to comment [{comment.Id}] in post [{post.Id}] added.");
                            }
                        }
                    }
                }
            }
        }

        public void PostsCopyingByLikesScenario()
        {
            if (!_isRunning) return;
            
            var unprocessedPosts = GetUnprocessedPosts();
            if (unprocessedPosts == null) return;

            foreach (var post in unprocessedPosts)
            {
                if (post.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                    _settings.LikeLimitForPostToCopy)
                {
                    if (!_processedObjectsRepository.DoesContainCopiedPostWith(post.Id))
                    {
                        var isSuccessful = _apiService.CreatePost(_spamRepository.GetRandomPostTitle(), post.Content);
                        if (isSuccessful)
                        {
                            _processedObjectsRepository.AddNewCopiedPost(post.Id);
                            _logger.Information($"Post [{post.Id}] was copied.");
                        }
                    }
                }
            }
        }

        public void PostsCopyingByUsersScenario()
        {
            if (!_isRunning) return;
            
            var unprocessedUsers = GetUnprocessedUsers();
            if (unprocessedUsers == null) return;

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
                    _logger.Information($"Like boosting restarted.");
                }
                return;
            }

            var user = _apiService.GetUser(_settings.UserId);

            if (user?.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                _settings.LikeLimitToCompleteProcess)
            {
                _isRunning = false;
                _logger.Information($"Like boosting completed.");
            }
        }

        private IEnumerable<Post> GetUnprocessedPosts()
        {
            var posts = _apiService.GetPosts();
            if (posts == null)
            {
                _logger.Information($"Posts were not loaded. The null-object was received.");
                return null;
            }

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
            if (users == null)
            {
                _logger.Information($"Users were not loaded. The null-object was received.");
                return null;
            }

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

            if (theMostLikedUserPost == null)
            {
                _logger.Information($"User was not loaded. The null-object was received.");
                return;
            }

            if (!_processedObjectsRepository.DoesContainCopiedPostWith(theMostLikedUserPost.Id))
            {
                var isSuccessful = _apiService.CreatePost(theMostLikedUserPost.Title, theMostLikedUserPost.Content);
                if (isSuccessful)
                {
                     _processedObjectsRepository.AddNewCopiedPost(theMostLikedUserPost.Id);
                     _logger.Information($"Post [{theMostLikedUserPost.Id}] was copied.");
                }
            }
        }

        private Post GetTheMostLikedUserPost(int userId)
        {
            var posts = _apiService.GetUserPosts(userId);

            return posts?.OrderByDescending(post => post.Likes.Length)
                .FirstOrDefault();
        }
    }
}