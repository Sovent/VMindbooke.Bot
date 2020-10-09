using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Application
{
    public class LikeBoostingService
    {
        private readonly BotSettings _settings;
        private readonly IAPIRequestsService _apiService;
        private readonly User _boostedUser;
        private DateTime _currentDate;
        private bool _isRunning;
        
        private readonly ISpamRepository _spamRepository;
        private readonly IProcessedObjectsRepository _processedObjectsRepository;

        private readonly ILogger _logger;

        public LikeBoostingService(BotSettings settings, IAPIRequestsService apiService, DateTime currentDate, User boostedUser, ISpamRepository spamRepository, IProcessedObjectsRepository processedObjectsRepository, ILogger logger)
        {
            _settings = settings;
            _apiService = apiService;
            _currentDate = currentDate;
            _boostedUser = boostedUser;
            _spamRepository = spamRepository;
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
                        var isSuccessful = _apiService.PostComment(post.Id, _spamRepository.GetRandomComment(), _boostedUser.Token);
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
                var allPostComments = GetAllComments(post.Comments);
                
                foreach (var comment in allPostComments)
                {
                    if (comment.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                        _settings.LikeLimitForCommentToMakeReply)
                    {
                        if (!_processedObjectsRepository.DoesContainRepliedCommentWith(comment.Id))
                        {
                            var isSuccessful = _apiService.ReplyToComment(post.Id, comment.Id, _spamRepository.GetRandomReply(), _boostedUser.Token);
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

        private IEnumerable<CommentToReply> GetAllComments(IEnumerable<Comment> comments)
        {
            var result = new List<CommentToReply>();

            foreach (var comment in comments)
            {
                result.Add(new CommentToReply(comment.Id, comment.Likes));
                if (comment.Replies.Length != 0)
                    result.AddRange(GetAllComments(comment.Replies));
            }

            return result;
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
                        var isSuccessful = _apiService.CreatePost(_boostedUser.Id, _boostedUser.Token, _spamRepository.GetRandomPostTitle(), post.Content);
                        if (isSuccessful)
                        {
                            _processedObjectsRepository.AddNewCopiedPost(post.Id);
                            _logger.Information($"Post [{post.Id}] was copied (title was changed).");
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

        private void CopyTheMostLikedPost(int userId)
        {
            var theMostLikedUserPost = GetTheMostLikedUserPost(userId);

            if (theMostLikedUserPost == null)
            {
                _logger.Information($"User's post was not loaded. The null-object was received.");
                return;
            }

            if (!_processedObjectsRepository.DoesContainCopiedPostWith(theMostLikedUserPost.Id))
            {
                var isSuccessful = _apiService.CreatePost(_boostedUser.Id, _boostedUser.Token, theMostLikedUserPost.Title, theMostLikedUserPost.Content);
                if (isSuccessful)
                {
                    _processedObjectsRepository.AddNewCopiedPost(theMostLikedUserPost.Id);
                    _logger.Information($"Post [{theMostLikedUserPost.Id}] was completely copied.");
                }
            }
        }

        private Post GetTheMostLikedUserPost(int userId)
        {
            var posts = _apiService.GetUserPosts(userId).GetValidCollection<Post>();

            return posts?.OrderByDescending(post => post.Likes.Length)
                .FirstOrDefault();
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

            var user = _apiService.GetUser(_boostedUser.Id);

            if (user?.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                _settings.LikeLimitToCompleteProcess)
            {
                _isRunning = false;
                _logger.Information($"Like boosting completed.");
            }
        }

        private IEnumerable<Post> GetUnprocessedPosts()
        {
            var posts = _apiService.GetPosts().GetValidCollection<Post>();
            if (posts == null)
            {
                _logger.Information($"Posts were not loaded. The null-object was received.");
                return null;
            }

            return posts;
        }

        private IEnumerable<User> GetUnprocessedUsers()
        {
            var users = _apiService.GetUsers().GetValidCollection<User>();
            if (users == null)
            {
                _logger.Information($"Users were not loaded. The null-object was received.");
                return null;
            }
            
            return users;
        }
    }
}