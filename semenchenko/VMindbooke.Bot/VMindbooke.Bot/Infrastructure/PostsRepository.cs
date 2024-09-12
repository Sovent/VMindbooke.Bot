using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Infrastructure
{
    public class PostsRepository : IPostsRepository
    {
        private readonly RestClient _restClient;

        public PostsRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public IReadOnlyCollection<Post> GetPosts()
        {
            var request = new RestRequest("posts", Method.GET);
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                }, (result, span) => Console.WriteLine());
            var response = retryPolicy.Execute(() => _restClient.Execute(request));

            var content = JsonConvert.DeserializeObject<Post[]>(response.Content);
            return content;
        }
    }
}