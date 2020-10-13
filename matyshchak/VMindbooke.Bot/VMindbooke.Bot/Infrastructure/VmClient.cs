using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using Serilog;
using Usage.Domain;
using Usage.Domain.Entities;
using Usage.Domain.ValueObjects;

namespace Usage.Infrastructure
{
    public class VmClient : IVmClient
    {
        private readonly RestClient _restClient;
        private readonly ILogger _logger;
        private readonly Policy<IRestResponse> _retryPolicy;
        
        public VmClient(VmClientUrl vmindbookeBaseUrl, ILogger logger)
        {
            _logger = logger;
            _restClient = new RestClient(vmindbookeBaseUrl.Value);
            _retryPolicy = CreatePolicyForInternalServerError();
        }

        private Policy<IRestResponse> CreatePolicyForInternalServerError()
        {
            return Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(5, times =>
                    TimeSpan.FromSeconds(times * 2), (result, span) =>
                    _logger.Warning(
                    $"{HttpStatusCode.InternalServerError} while executing request, next retry in {span}."));
        }

        public User Register(UserName userName)
        {
            var request = new RestRequest($"users/", Method.POST);
            request.AddJsonBody(userName);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var user = JsonConvert.DeserializeObject<User>(response.Content);
            _logger.Information($"Successfully registered a user with id: {user.Id}");
            return user;
        }
        
        public IReadOnlyCollection<User> GetAllUsers()
        {
            var request = new RestRequest($"users", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var users = JsonConvert.DeserializeObject<User[]>(response.Content);
            _logger.Information($"Successfully got all users");
            return users;
        }
        
        public IReadOnlyCollection<User> GetUsers(int take, int skip = 0)
        {
            var request = new RestRequest($"users/?take={take}&skip={skip}", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var users = JsonConvert.DeserializeObject<User[]>(response.Content);
            _logger.Information($"Successfully got all users");
            return users;
        }
        
        public User GetUser(int userId)
        {
            var request = new RestRequest($"users/{userId}", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var user = JsonConvert.DeserializeObject<User>(response.Content);
            _logger.Information($"Successfully got user with id: {userId}");
            return user;
        }
        
        public IReadOnlyCollection<Post> GetUserPosts(int userId)
        {
            var request = new RestRequest($"user/{userId}/posts", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var posts = JsonConvert.DeserializeObject<Post[]>(response.Content);
            _logger.Information($"Successfully got posts of user with id: {userId}");
            return posts;
        }
        
        public IEnumerable<Post> GetAllPosts()
        {
            var request = new RestRequest($"posts/", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var posts = JsonConvert.DeserializeObject<Post[]>(response.Content);
            _logger.Information($"Successfully got all posts");
            return posts;
        }
        
        public IEnumerable<Comment> GetAllComments()
        {
            return GetAllPosts().SelectMany(post => post.Comments);
        }
        
        public IReadOnlyCollection<Post> GetPosts(int take, int skip = 0)
        {
            var request = new RestRequest($"posts/?take={take}&skip={skip}", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<Post[]>(response.Content);
            return content;
        }

        public int Post(int userId, string userToken, PostRequest postRequest)
        {
            var request = new RestRequest($"users/{userId}/posts", Method.POST);
            request.AddHeader("Authorization", userToken);
            request.AddJsonBody(postRequest);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var postId = int.Parse(response.Content);
            _logger.Information($"Successfully added new post with id: {postId}.");
            return postId;
        }

        public void CommentPost(int userId, string userToken, int postId, CommentContent comment)
        {
            var request = new RestRequest($"/posts/{postId}/comments", Method.POST);
            request.AddHeader("Authorization", userToken);
            request.AddJsonBody(comment);
            _retryPolicy.Execute(() => _restClient.Execute(request));
            _logger.Information($"Successfully commented post with id: {postId}.");
        }
        
        public void ReplyToComment(string userToken, int postId, Guid commentId, CommentContent reply)
        {
            var request = new RestRequest($"/posts/{postId}/comments/{commentId}/replies", Method.POST);
            request.AddHeader("Authorization", userToken);
            request.AddJsonBody(reply);
            _retryPolicy.Execute(() => _restClient.Execute(request));
            _logger.Information($"Successfully replied to comment with id: {commentId}.");
        }
    }
}