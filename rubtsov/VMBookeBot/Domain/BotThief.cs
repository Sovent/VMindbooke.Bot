using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using VMBook.SDK;

namespace VMBookeBot.Domain
{
    public class BotThief
    {
        private readonly List<int> _alreadyStolenPosts;
        private readonly List<Guid> _alreadyRepliedComments;
        private readonly List<int> _alreadyCommentedPosts;
        private readonly IRequestRetryer _requestsRetryer;
        private readonly IRequestFilter _requestsFilter;

        public BotThief(IRequestRetryer requestsRetryer, 
            IRequestFilter requestsFilter,
            IUserActivityRepository currentUserActivityRepository
            )
        {
            _requestsRetryer = requestsRetryer;
            _requestsFilter = requestsFilter;
            _alreadyStolenPosts = currentUserActivityRepository.AlreadyStolenPosts;
            _alreadyRepliedComments = currentUserActivityRepository.AlreadyRepliedComments;
            _alreadyCommentedPosts = currentUserActivityRepository.AlreadyCommentedPosts;
        }

        public void WriteCommentUnderPopularPosts(IReadOnlyCollection<Post> posts)
        {
            foreach (var post in posts)
            {
                if (_alreadyCommentedPosts.Contains(post.Id))
                {
                    Log.Debug($"Skipped post {post.Id}");
                    continue;
                }
                var isWritten = _requestsRetryer.TryWriteComment(post.Id);
                if (isWritten)
                {
                    _alreadyCommentedPosts.Add(post.Id);
                    Log.Debug("Added post in list");
                }
            }
        }

        public void WriteReplyUnderPopularComment(IReadOnlyCollection<Post> posts, int minLikesNumber)
        {
            foreach (var post in posts)
            {
                if (!post.Comments.Any()) continue;
                foreach (var comment in post)
                {
                    if (comment.Likes.Count < minLikesNumber) continue;
                    if (_alreadyRepliedComments.Contains(comment.Id)) continue;
                    var isWritten = _requestsRetryer.TryWriteReplyUnderComment(post.Id, comment.Id);
                    if (isWritten)
                    {
                        _alreadyRepliedComments.Add(comment.Id);
                    }
                }
            }
        }

        public void StealPopularPost(IReadOnlyCollection<Post> posts, int minLikesNumber)
        {
            foreach (var post in posts)
            {
                if (post.Likes.Count < minLikesNumber) continue;
                if (_alreadyStolenPosts.Contains(post.Id)) continue;
                var isCopied = _requestsRetryer.TryMakeCopyOfPost("Stolen post", post.Content, post.Id);
                if (isCopied)
                {
                    _alreadyStolenPosts.Add(post.Id);
                }
            }
        }

        public void StealPostFromPopularUser(IReadOnlyCollection<User> popularUsers)
        {
            foreach (var user in popularUsers)
            {
                var post = _requestsFilter.GetMostPopularUserPost(user.Id);
                if (_alreadyStolenPosts.Contains(post.Id)) continue;
                var isCopied = _requestsRetryer.TryMakeCopyOfPost("stolen post", post.Content, post.Id);
                if (isCopied)
                {
                    _alreadyStolenPosts.Add(post.Id);
                }
            }
        }

        public bool IsCurrentUserLikesLimitExceeded(int userId, int minLikesNumber)
        {
            var user = _requestsFilter.GetUser(userId);
            return user.Likes.Count > minLikesNumber;
        }
    }
}