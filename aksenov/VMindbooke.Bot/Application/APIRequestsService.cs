using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;
using Serilog.Core;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Application
{
    public class APIRequestsService
    {
        private RestClient _restClient;
        private RetryPolicy<IRestResponse> _retryPolicy;
        private readonly BotSettings _settings;
        private readonly Logger _logger;

        public APIRequestsService(BotSettings settings, Logger logger)
        {
            _settings = settings;
            _logger = logger;
            Initialize();
        }

        private void Initialize()
        {
            _restClient = new RestClient(_settings.ServerAddress);
            _retryPolicy = Policy<IRestResponse>
                .HandleResult(response => !response.IsSuccessful)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                });
        }
        
        private IEnumerable<Post> TryGetPosts()
        {
            var resource = "posts";
            var request = new RestRequest(resource, Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<Post[]>(response.Content);
            return content;
        }

        private IEnumerable<User> TryGetUsers()
        {
            var resource = "users";
            var request = new RestRequest(resource, Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<User[]>(response.Content);
            return content;
        }

        private IEnumerable<Post> TryGetUserPosts(int userId)
        {
            var resource = $"users/{userId}/posts";
            var request = new RestRequest(resource, Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<Post[]>(response.Content);
            return content;
        }

        private User TryGetUser(int userId)
        {
            var resource = $"users/{userId}";
            var request = new RestRequest(resource, Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<User>(response.Content);
            return content;
        }

        private bool TryPostComment(int postId, string content)
        {
            var resource = $"posts/{postId}/comments";
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(new CommentRequest(content));
            request.AddHeader("Authorization", _settings.UserToken);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            return response.IsSuccessful;
        }

        private bool TryReplyToComment(int postId, string commentId, string content)
        {
            var resource = $"posts/{postId}/comments/{commentId}/replies";
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(new CommentRequest(content));
            request.AddHeader("Authorization", _settings.UserToken);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            return response.IsSuccessful;
        }

        private bool TryCreatePost(string title, string content)
        {
            var resource = $"users/{_settings.UserId}/posts";
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(new PostRequest(title, content));
            request.AddHeader("Authorization", _settings.UserToken);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            return response.IsSuccessful;
        }

        public IEnumerable<Post> GetPosts()
        {
            try
            {
                return TryGetPosts();
            }
            catch (Exception e)
            {
                _logger.Error($"An error occurred while loading posts: {e.Message}");
                return null;
            }
        }

        public IEnumerable<User> GetUsers()
        {
            try
            {
                return TryGetUsers();
            }
            catch (Exception e)
            {
                _logger.Error($"An error occurred while loading users: {e.Message}");
                return null;
            }
        }

        public IEnumerable<Post> GetUserPosts(int userId)
        {
            try
            {
                return TryGetUserPosts(userId);
            }
            catch (Exception e)
            {
                _logger.Error($"An error occurred while loading user's posts: {e.Message}");
                return null;
            }
        }

        public User GetUser(int userId)
        {
            try
            {
                return TryGetUser(userId);
            }
            catch (Exception e)
            {
                _logger.Error($"An error occurred while loading a user: {e.Message}");
                return null;
            }
        }

        public bool PostComment(int postId, string content)
        {
            try
            {
                return TryPostComment(postId, content);
            }
            catch (Exception e)
            {
                _logger.Error($"An error occurred while posting a comment: {e.Message}");
                return false;
            }
        }

        public bool ReplyToComment(int postId, string commentId, string content)
        {
            try
            {
                return TryReplyToComment(postId, commentId, content);
            }
            catch (Exception e)
            {
                _logger.Error($"An error occurred while replying to a comment: {e.Message}");
                return false;
            }
        }

        public bool CreatePost(string title, string content)
        {
            try
            {
                return TryCreatePost(title, content);
            }
            catch (Exception e)
            {
                _logger.Error($"An error occurred while creating a post: {e.Message}");
                return false;
            }
        }
    }
}