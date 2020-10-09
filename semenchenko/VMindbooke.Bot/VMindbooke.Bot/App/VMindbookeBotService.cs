using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.App
{
    public class VMindbookeBotService : IVMindbookeBotService
    {
        private readonly IActionMaker _actionMaker;

        private readonly History<int> _commentedPostsHistory;

        private readonly IPostsRepository _postsRepository;

        private readonly PostsProcessor _processor;
        private readonly History<string> _repliedCommentsHistory;
        private readonly History<int> _repostedOrCopiedPostsHistory;
        private readonly RestClient _restClient;

        public VMindbookeBotService(
            IConfigurationRoot configuration,
            IActionMaker actionMaker,
            IPostsRepository postsRepository,
            RestClient client)
        {
            _processor = new PostsProcessor(configuration);
            _actionMaker = actionMaker;
            _postsRepository = postsRepository;
            _restClient = client;

            _commentedPostsHistory = new History<int>();
            _repostedOrCopiedPostsHistory = new History<int>();
            _repliedCommentsHistory = new History<string>();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<Post> GetPosts()
        {
            return _postsRepository.GetPosts();
        }

        public IEnumerable<Comment> GetComments(Post post)
        {
            var request = new RestRequest("posts/" + post.Id + "/comments", Method.GET);
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                }, (result, span) => Console.WriteLine());
            var response = retryPolicy.Execute(() => _restClient.Execute(request));

            var content = JsonConvert.DeserializeObject<Comment[]>(response.Content);
            return content;
        }

        public void CommentIfLiked(Post post)
        {
            if (!_commentedPostsHistory.IsStored(post.Id) && _processor.IsPostCommentable(post))
                _actionMaker.CommentPost(post);
        }

        public void ReplyIfLiked(Post post, Comment comment)
        {
            if (!_repliedCommentsHistory.IsStored(comment.Id) && _processor.IsCommentReplyable(comment))
                _actionMaker.ReplyToComment(post, comment);
        }

        public void RepostIfLiked(Post post)
        {
            if (!_repostedOrCopiedPostsHistory.IsStored(post.Id) && _processor.IsPostRepostable(post))
                _actionMaker.RepostPost(post);
        }

        public void CopyIfLiked(Post post)
        {
            if (!_repostedOrCopiedPostsHistory.IsStored(post.Id) && _processor.IsPostCopyable(post))
                _actionMaker.CopyPost(post);
        }

        public bool IsLikedEnough()
        {
            throw new NotImplementedException();
        }
    }
}