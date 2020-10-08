using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using VMindbooke.SDK;
using VMindbookeClient.Domain;

namespace VMindbookeClient
{
    public class VMindbookeClient : IVMindbookeClient
    {
        private readonly RestClient _restClient;

        public VMindbookeClient(string vmindbookeBaseUrl)
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
            
            var content = JsonConvert.DeserializeObject<Post[]>(response.Content);
            return content;
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

        public int Post(int userId, string userToken, PostContent postContent)
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
            request.AddJsonBody(postContent);

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