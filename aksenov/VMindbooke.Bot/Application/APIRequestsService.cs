using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Application
{
    public class APIRequestsService
    {
        private RestClient _restClient;
        private RetryPolicy<IRestResponse> _retryPolicy;
        private readonly BotSettings _settings;

        public APIRequestsService(BotSettings settings)
        {
            _settings = settings;
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
        
        public IEnumerable<Post> GetPosts()
        {
            var resource = "posts";
            var request = new RestRequest(resource, Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<Post[]>(response.Content);
            return content;
        }
        
        public IEnumerable<User> GetUsers()
        {
            var resource = "users";
            var request = new RestRequest(resource, Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<User[]>(response.Content);
            return content;
        }
        
        public IEnumerable<Post> GetUserPosts(int userId)
        {
            var resource = $"users/{userId}/posts";
            var request = new RestRequest(resource, Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<Post[]>(response.Content);
            return content;
        }
        
        public User GetUser(int userId)
        {
            var resource = $"users/{userId}";
            var request = new RestRequest(resource, Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<User>(response.Content);
            return content;
        }
        
        public bool PostComment(int postId, string content)
        {
            var resource = $"posts/{postId}/comments";
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(new CommentRequest(content));
            request.AddHeader("Authorization", _settings.UserToken);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            return response.IsSuccessful;
        }
        
        public bool ReplyToComment(int postId, string commentId, string content)
        {
            var resource = $"posts/{postId}/comments/{commentId}/replies";
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(new CommentRequest(content));
            request.AddHeader("Authorization", _settings.UserToken);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            return response.IsSuccessful;
        }

        public bool CreatePost(string title, string content)
        {
            var resource = $"users/{_settings.UserId}/posts";
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(new PostRequest(title, content));
            request.AddHeader("Authorization", _settings.UserToken);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            return response.IsSuccessful;
        }
    }
}