using System;
using System.Net;
using Polly;
using RestSharp;
using Serilog;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Infrastructure
{
    public class ActionMaker : IActionMaker
    {
        private readonly RestClient _client;
        private readonly IShitPostFactory _factory;
        private readonly string _token;
        private readonly int _id;
        private readonly Policy<IRestResponse> retryPolicy;

        public ActionMaker(IShitPostFactory factory, RestClient client, string token, int userId)
        {
            _token = token;
            _client = client;
            _factory = factory;
            _id = userId;
            retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                }, (result, span) => Log.Information(result.Result.Content));
        }

        public void CommentPost(Post post)
        {
            var comment = _factory.GenerateComment();
            var request = new RestRequest($"posts/{post.Id}/comments", Method.POST);
            request.AddJsonBody(comment);
            request.AddHeader("Authorization", _token);

            retryPolicy.Execute(() => _client.Execute(request));
        }

        public void ReplyToComment(Post post, Comment comment)
        {
            var commentText = _factory.GenerateComment();
            var request = new RestRequest($"posts/{post.Id}/comments/{comment.Id}/replies", Method.POST);
            request.AddJsonBody(commentText);
            request.AddHeader("Authorization", _token);

            retryPolicy.Execute(() => _client.Execute(request));
        }

        public void RepostPost(Post post)
        {
            var title = _factory.GenerateTitle();
            var request = new RestRequest($"user/{_id}/posts", Method.POST);
            request.AddJsonBody("header", title);
            request.AddJsonBody("content", post.Content);
            request.AddHeader("Authorization", _token);

            retryPolicy.Execute(() => _client.Execute(request));
        }

        public void CopyPost(Post post)
        {
            var title = _factory.GenerateTitle();
            var request = new RestRequest($"user/{_id}/posts", Method.POST);
            request.AddJsonBody("title", post.Title);
            request.AddJsonBody("content", post.Content);
            request.AddHeader("Authorization", _token);

            retryPolicy.Execute(() => _client.Execute(request));
        }
    }
}