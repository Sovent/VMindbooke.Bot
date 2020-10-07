using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using VMindBooke.SDK.Domain;

namespace VMindBooke.SDK.Application
{
    public class PostService : IPostService
    {
        private RestClient _restClient;
        private Policy<IRestResponse> _retryPolicy;
        
        // Set retry pow to 2 before prod!
        public PostService(RestClient restClient)
        {
            _restClient = restClient;
            _retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(5, retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)) );
        }

        public Post GetPost(int id)
        {
            var request = new RestRequest($"posts/{id}", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<Post>(response.Content);
            return content;
        }
        
        public IReadOnlyCollection<Post> GetAllPosts()
        {
            var request = new RestRequest("posts", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<IReadOnlyCollection<Post>>(response.Content);
            return content;
        }

        public void LikePost(Post post, User actor)
        {
            var request = new RestRequest($"posts/{post.Id}/likes", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
        }

        public void CommentPost(Post post, User actor, string content)
        {
            var request = new RestRequest($"post/{post.Id}/comments", Method.POST);
            request.AddJsonBody(
                new 
                {
                    content = content
                });
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
        }

        public void ReplyComment(Post post, Comment comment, User actor, string content)
        {
            var request = new RestRequest($"post/{post.Id}/comments/{comment.Id}/replies", Method.POST);
            request.AddJsonBody(
                new 
                {
                    content = content
                });
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
        }

        public void LikeComment(Post post, Comment comment, User actor)
        {
            var request = new RestRequest($"post/{post.Id}/comments/{comment.Id}/likes", Method.POST);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
        }
    }
}