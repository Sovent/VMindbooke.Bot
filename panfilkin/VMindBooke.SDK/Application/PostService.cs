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
        private readonly RestClient _restClient;
        private readonly Policy<IRestResponse> _retryPolicy;

        public PostService(RestClient restClient)
        {
            _restClient = restClient;
            _retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode != HttpStatusCode.OK)
                .WaitAndRetry(15, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)));
        }

        public Post GetPost(int id)
        {
            var request = new RestRequest($"posts/{id}", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<Post>(response.Content);
            return content;
        }

        public IEnumerable<Post> GetAllPosts()
        {
            var request = new RestRequest("posts", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<List<Post>>(response.Content);
            return content ?? new List<Post>();
        }

        public void LikePost(Post post, User actor)
        {
            var request = new RestRequest($"posts/{post.Id}/likes", Method.POST);
            request.AddHeader("Authorization", actor.Token);
            _retryPolicy.Execute(() => _restClient.Execute(request));
        }

        public void CommentPost(Post post, User actor, string content)
        {
            var request = new RestRequest($"posts/{post.Id}/comments", Method.POST);
            request.AddHeader("Authorization", actor.Token);
            request.AddJsonBody(
                new
                {
                    content
                });
            _retryPolicy.Execute(() => _restClient.Execute(request));
        }

        public void ReplyComment(Post post, Comment comment, User actor, string content)
        {
            var request = new RestRequest($"posts/{post.Id}/comments/{comment.Id}/replies", Method.POST);
            request.AddHeader("Authorization", actor.Token);
            request.AddJsonBody(
                new
                {
                    content
                });
            _retryPolicy.Execute(() => _restClient.Execute(request));
        }

        public void LikeComment(Post post, Comment comment, User actor)
        {
            var request = new RestRequest($"posts/{post.Id}/comments/{comment.Id}/likes", Method.POST);
            request.AddHeader("Authorization", actor.Token);
            _retryPolicy.Execute(() => _restClient.Execute(request));
        }
    }
}