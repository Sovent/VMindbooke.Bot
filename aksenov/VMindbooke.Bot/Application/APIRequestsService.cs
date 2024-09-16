using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;
using Serilog;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Application
{
    public class APIRequestsService : IAPIRequestsService
    {
        private RestClient _restClient;
        private RetryPolicy<IRestResponse> _retryPolicy;
        private readonly string _serverAddress;
        private readonly ILogger _logger;

        public APIRequestsService(string serverAddress, ILogger logger)
        {
            _logger = logger;
            _serverAddress = serverAddress;
            Initialize();
        }

        private void Initialize()
        {
            _restClient = new RestClient(_serverAddress);
            _retryPolicy = Policy<IRestResponse>
                .HandleResult(response => !response.IsSuccessful)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }, (result, span) => _logger.Information($"Retry for request ({result.Result.Request.Method.ToString()}) with resource ({result.Result.Request.Resource})"));
        }

        private T MakeGetRequest<T>(string resource)
        {
            var request = new RestRequest(resource, Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<T>(response.Content);
            return content;
        }

        private IRestResponse MakePostRequest(string resource, Object requestObject, ICollection<KeyValuePair<string, string>> headers)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(requestObject);
            request.AddHeaders(headers);
            return _retryPolicy.Execute(() => _restClient.Execute(request));
        }

        public IEnumerable<Post> GetPosts()
        {
            try
            {
                return MakeGetRequest<IEnumerable<Post>>($"posts");
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
                return MakeGetRequest<IEnumerable<User>>($"users");
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
                return MakeGetRequest<IEnumerable<Post>>($"users/{userId}/posts");
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
                return MakeGetRequest<User>($"users/{userId}");
            }
            catch (Exception e)
            {
                _logger.Error($"An error occurred while loading a user: {e.Message}");
                return null;
            }
        }

        public bool PostComment(int postId, string content, string userToken)
        {
            try
            {
                return MakePostRequest(
                        $"posts/{postId}/comments",
                        new CommentRequest(content),
                        new Dictionary<string, string>()
                        {
                            {"Authorization", userToken}
                        })
                    .IsSuccessful;
            }
            catch (Exception e)
            {
                _logger.Error($"An error occurred while posting a comment: {e.Message}");
                return false;
            }
        }

        public bool ReplyToComment(int postId, string commentId, string content, string userToken)
        {
            try
            {
                return MakePostRequest(
                        $"posts/{postId}/comments/{commentId}/replies",
                        new CommentRequest(content),
                        new Dictionary<string, string>()
                        {
                            {"Authorization", userToken}
                        })
                    .IsSuccessful;
            }
            catch (Exception e)
            {
                _logger.Error($"An error occurred while replying to a comment: {e.Message}");
                return false;
            }
        }

        public bool CreatePost(int userId, string userToken, string title, string content)
        {
            try
            {
                return MakePostRequest(
                        $"users/{userId}/posts",
                        new PostRequest(title, content),
                        new Dictionary<string, string>()
                        {
                            {"Authorization", userToken}
                        })
                    .IsSuccessful;
            }
            catch (Exception e)
            {
                _logger.Error($"An error occurred while creating a post: {e.Message}");
                return false;
            }
        }

        public User CreateUser(string name)
        {
            try
            {
                var response = MakePostRequest(
                    $"users",
                    new UserRequest(name),
                    new Dictionary<string, string>());

                return JsonConvert.DeserializeObject<User>(response.Content);
            }
            catch (Exception e)
            {
                _logger.Error($"An error occurred while creating a user: {e.Message}");
                return null;
            }
        }
    }
}