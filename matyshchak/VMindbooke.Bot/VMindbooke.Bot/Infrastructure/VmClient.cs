using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using Usage.Domain;
using Usage.Domain.ContentProviders;
using Usage.Domain.Entities;

namespace Usage.Infrastructure
{
    public class VmClient : IVmClient
    {
        private readonly RestClient _restClient;

        public VmClient(string vmindbookeBaseUrl)
        {
            _restClient = new RestClient(vmindbookeBaseUrl);
        }

        public User Register(UserName userName)
        {
            var request = new RestRequest($"users/", Method.POST);
            request.AddJsonBody(userName);
            var response = _restClient.Execute(request);
            
            var content = JsonConvert.DeserializeObject<User>(response.Content);
            return content;
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
                }, (result, span) => Console.WriteLine());
            
            var request = new RestRequest($"users", Method.GET);
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            var content = JsonConvert.DeserializeObject<User[]>(response.Content);
            return content;
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
                }, (result, span) => Console.WriteLine());
            
            var request = new RestRequest($"users/?take={take}&skip={skip}", Method.GET);
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            var content = JsonConvert.DeserializeObject<User[]>(response.Content);
            return content;
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
                }, (result, span) => Console.WriteLine());
            
            var request = new RestRequest($"users/{userId}", Method.GET);
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            var content = JsonConvert.DeserializeObject<User>(response.Content);
            return content;
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
                }, (result, span) => Console.WriteLine());
            
            var request = new RestRequest($"user/{userId}/posts", Method.GET);
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            var posts = JsonConvert.DeserializeObject<Post[]>(response.Content);
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
                }, (result, span) => Console.WriteLine());
            
            var request = new RestRequest($"posts/", Method.GET);
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            var posts = JsonConvert.DeserializeObject<Post[]>(response.Content);
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
                }, (result, span) => Console.WriteLine());
            
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
                }, (result, span) => Console.WriteLine());

            var request = new RestRequest($"users/{userId}/posts", Method.POST);
            request.AddHeader("Authorization", userToken);
            request.AddJsonBody(postRequest);

            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<int>(response.Content);
            return content;
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
                }, (result, span) => Console.WriteLine());
            
            var request = new RestRequest($"/posts/{postId}/comments", Method.POST);
            request.AddHeader("Authorization", userToken);
            request.AddJsonBody(comment);

            var response = retryPolicy.Execute(() => _restClient.Execute(request));
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
                }, (result, span) => Console.WriteLine());
            
            var request = new RestRequest($"/posts/{postId}/comments/{commentId}/replies", Method.POST);
            request.AddHeader("Authorization", userToken);
            request.AddJsonBody(reply);

            var response = retryPolicy.Execute(() => _restClient.Execute(request));
        }
        
    }
}