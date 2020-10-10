using System;
using System.Collections.Generic;
using System.Net.Cache;
using System.Net.Http;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Polly;
using Polly.Wrap;
using RestSharp;
using Serilog;

namespace VMindbookeClient
{
    public class HttpException : Exception
    {
        
    }
    
    public interface IApiClient
    {
        IReadOnlyCollection<User> GetUsers(int take = Int32.MaxValue, int skip = 0);
        IReadOnlyCollection<Post> GetPosts(int take = Int32.MaxValue, int skip = 0);
        IReadOnlyCollection<Post> GetUserPosts(int userId);
        User GetUser(int userId);
        Post GetPost(int userId);

        UserAuthInfo CreateUser(string name);
        
        void CreateComment(UserAuthInfo user, int postId, string content);
        void LikePost(UserAuthInfo user, int postId);
        void LikeComment(UserAuthInfo user, int postId, string commentId);
        void ReplyComment(UserAuthInfo user, int postId, string commentId, string content);
        void CreatePost(UserAuthInfo user, string content, string title);
    }
    
    
    public class ApiClient : IApiClient
    {
        private string _address;
        private int _retryCount;
        
        private RestClient _client;

        private PolicyWrap<IRestResponse> _policyWrap;

        private void SetClientParameters()
        {
            _client = new RestClient(_address);
            _client.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
            
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => !r.IsSuccessful)
                .Retry(_retryCount, onRetry: (result, retryCount) =>
                {
                    Log.Warning($"Request failed with {result.Result.StatusCode}. Retry attempt {retryCount}");
                });

            var breakPolicy = Policy<IRestResponse>
                .Handle<HttpRequestException>()
                .Or<OperationCanceledException>()
                .Fallback(() =>
                {
                    Log.Error("Error in using http requests occured");
                    throw new HttpException();
                });
            
            _policyWrap = Policy.Wrap( retryPolicy, breakPolicy);
        }
        public ApiClient(string address, int retryCount = 3)
        {
            _address = address ?? throw new ArgumentNullException(nameof(address));
            _retryCount = retryCount;
        }

        private T GetRequest<T>(string apiPath)
        {
            var request = new RestRequest(apiPath, Method.GET);
            Log.Information($"Try execute GET Request to {_address} for {apiPath}");
           
            var response = _policyWrap.Execute(() => _client.Execute(request));

            if (!response.IsSuccessful)
            {
                Log.Error($"Response failed. Status code {response.StatusCode}");
                throw new HttpException();
            }

            var content = JsonConvert.DeserializeObject<T>(response.Content);
            return content;
        }
        private void PostRequest(string apiPath, string authToken, object content = null)
        {
            var request = new RestRequest(apiPath, Method.POST);
            Log.Information($"Try execute POST Request to {_address} for {apiPath}");
            
            request.AddHeader("Authorization", authToken);
            if (content != null)
                request.AddJsonBody(content);

            var response = _policyWrap.Execute(() => _client.Execute(request));

            if (!response.IsSuccessful)
            {
                Log.Error($"Response failed. Status code {response.StatusCode}");
                throw new HttpException();
            }
        }
        
        public IReadOnlyCollection<User> GetUsers(int take = Int32.MaxValue, int skip = 0)
        {
            return GetRequest<List<User>>($"users?take={take}&skip={skip}");
        }

        public IReadOnlyCollection<Post> GetPosts(int take = Int32.MaxValue, int skip = 0)
        {
            return GetRequest<List<Post>>($"posts?take={take}&skip={skip}");
        }

        public IReadOnlyCollection<Post> GetUserPosts(int userId)
        {
            return GetRequest<List<Post>>($"users/{userId}/posts");
        }

        public User GetUser(int userId)
        {
            return GetRequest<User>($"users/{userId}");
        }

        public Post GetPost(int postId)
        {
            return GetRequest<Post>($"posts/{postId}");
        }

        public UserAuthInfo CreateUser(string name)
        {
            throw new NotImplementedException();
        }

        public void CreateComment(UserAuthInfo user, int postId, string content)
        {
            PostRequest($"posts/{postId}/comments", 
                user.Token,
                new { Content = content });
        }

        public void LikePost(UserAuthInfo user, int postId)
        {
            PostRequest($"posts/{postId}/likes", user.Token);
        }

        public void LikeComment(UserAuthInfo user, int postId, string commentId)
        {
            PostRequest($"posts/{postId}/comments/{commentId}/likes", user.Token);
        }

        public void ReplyComment(UserAuthInfo user, int postId, string commentId, string content)
        {
            PostRequest($"posts/{postId}/comments/{commentId}/replies", 
                user.Token, 
                new { Content = content });
        }

        public void CreatePost(UserAuthInfo user, string content, string title)
        {
            PostRequest($"posts", 
                user.Token, 
                new { Content = content, Title = title });
        }
    }
}