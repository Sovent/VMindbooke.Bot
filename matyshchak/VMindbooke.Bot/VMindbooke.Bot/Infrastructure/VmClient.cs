using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using Serilog;
using Usage.Domain;
using Usage.Domain.ContentProviders;
using Usage.Domain.Entities;
using Usage.Domain.ValueObjects;

namespace Usage.Infrastructure
{
    public class VmClient : IVmClient
    {
        private readonly RestClient _restClient;
        private readonly ILogger _logger;
        
        public VmClient(string vmindbookeBaseUrl, ILogger logger)
        {
            _logger = logger;
            _restClient = new RestClient(vmindbookeBaseUrl);
        }

        public User Register(UserName userName)
        {
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                },
                    (result,
                        span) => Log.Warning(
                        $"{result.Exception.Message} while registering user with name {userName}."));
            
            var request = new RestRequest($"users/", Method.POST);
            request.AddJsonBody(userName);
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            var user = JsonConvert.DeserializeObject<User>(response.Content);
            Log.Information($"Successfully registered a user with id: {user.Id}");
            return user;
        }
        
        public IReadOnlyCollection<User> GetAllUsers()
        {
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                },
                    (result,
                        span) => Log.Information(
                        $"Failed to get posts. HttpStatusCode: {HttpStatusCode.InternalServerError}"));
            
            var request = new RestRequest($"users", Method.GET);
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            var users = JsonConvert.DeserializeObject<User[]>(response.Content);
            Log.Information($"Successfully got all users");
            return users;
        }
        
        public IReadOnlyCollection<User> GetUsers(int take, int skip = 0)
        {
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                }, (result, span) => Log.Warning($"Failed to get users. HttpStatusCode: {HttpStatusCode.InternalServerError}"));

            
            var request = new RestRequest($"users/?take={take}&skip={skip}", Method.GET);
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            var users = JsonConvert.DeserializeObject<User[]>(response.Content);
            Log.Information($"Successfully got all users");
            return users;
        }
        
        public User GetUser(int userId)
        {
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                },
                    (result,
                        span) => Log.Warning(
                        $"Failed to get user with id {userId}. HttpStatusCode: {HttpStatusCode.InternalServerError}"));

            var request = new RestRequest($"users/{userId}", Method.GET);
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            var user = JsonConvert.DeserializeObject<User>(response.Content);
            Log.Information($"Successfully got user with id: {userId}");
            return user;
        }
        
        public IReadOnlyCollection<Post> GetUserPosts(int userId)
        {
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                },
                    (result,
                        span) => Log.Warning(
                        $"Failed to get posts. HttpStatusCode: {HttpStatusCode.InternalServerError}"));

            
            var request = new RestRequest($"user/{userId}/posts", Method.GET);
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            var posts = JsonConvert.DeserializeObject<Post[]>(response.Content);
            Log.Information($"Successfully got posts of user with id: {userId}");
            return posts;
        }
        
        public IEnumerable<Post> GetAllPosts()
        {
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                },
                    (result,
                        span) => Log.Warning(
                        $"Failed to get posts. HttpStatusCode: {HttpStatusCode.InternalServerError}"));
            
            var request = new RestRequest($"posts/", Method.GET);
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            var posts = JsonConvert.DeserializeObject<Post[]>(response.Content);
            Log.Information($"Successfully got all posts");
            return posts;
        }
        
        public IEnumerable<Comment> GetAllComments()
        {
            return GetAllPosts().SelectMany(post => post.Comments);
        }
        
        public IReadOnlyCollection<Post> GetPosts(int take, int skip = 0)
        {
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                }, (result, span) => Log.Warning(
                    $"Failed to get posts. HttpStatusCode: {HttpStatusCode.InternalServerError}"));
            
            var request = new RestRequest($"posts/?take={take}&skip={skip}", Method.GET);
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            var content = JsonConvert.DeserializeObject<Post[]>(response.Content);
            return content;
        }

        public int Post(int userId, string userToken, PostRequest postRequest)
        {
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }, (result, span) => Log.Warning(
                    $"Failed to add a new post. HttpStatusCode: {HttpStatusCode.InternalServerError}"));

            var request = new RestRequest($"users/{userId}/posts", Method.POST);
            request.AddHeader("Authorization", userToken);
            request.AddJsonBody(postRequest);

            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            var postId = int.Parse(response.Content);
            Log.Information($"Successfully added new post with id: {postId}.");
            return postId;
        }

        public void CommentPost(int userId, string userToken, int postId, CommentContent comment)
        {
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                },
                    (result,
                        span) => Log.Warning(
                        $"Failed to comment post with id: {postId}. HttpStatusCode: {HttpStatusCode.InternalServerError}"));
            
            var request = new RestRequest($"/posts/{postId}/comments", Method.POST);
            request.AddHeader("Authorization", userToken);
            request.AddJsonBody(comment);

            retryPolicy.Execute(() => _restClient.Execute(request));
            Log.Information($"Successfully commented post with id: {postId}.");
        }
        
        public void ReplyToComment(string userToken, int postId, Guid commentId, CommentContent reply)
        {
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                },
                    (result,
                        span) => Log.Warning(
                        $"Failed to reply to comment with id: {commentId}. HttpStatusCode: {HttpStatusCode.InternalServerError}"));
            
            var request = new RestRequest($"/posts/{postId}/comments/{commentId}/replies", Method.POST);
            request.AddHeader("Authorization", userToken);
            request.AddJsonBody(reply);

            retryPolicy.Execute(() => _restClient.Execute(request));
            Log.Information($"Successfully replied to comment with id: {commentId}.");
        }
        
    }
}