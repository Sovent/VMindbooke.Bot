using System;
using Polly;
using Polly.Retry;
using Serilog;
using VMindbookeBotSDK;

namespace VMindbookeBot
{
    public class Policies
    {
        private BotService _botService;

        public Policies(BotService botService)
        {
            _botService = botService;
        }

        public RetryPolicy<Result<int, HttpRequester.StatusCode>> GetCreateNewPostPolicy(int userId)
        {
            return Policy<Result<int, HttpRequester.StatusCode>>
                .HandleResult(r => !r.HasValue).WaitAndRetry(_botService.Config.Retry, PolynomiallyTimeSpan,
                (r, span) =>
                    Log.Warning(
                        $"Retry Create New Post {nameof(userId)}={userId} : Time span {span}"));
        }

        public RetryPolicy<HttpRequester.StatusCode> GetWriteCommentPolicy(int postId)
        {
            return Policy<HttpRequester.StatusCode>
                .HandleResult(p => p.IsPostError()).WaitAndRetry(_botService.Config.Retry, PolynomiallyTimeSpan,
                (r, span) =>
                    Log.Warning(
                        $"Retry Write Comment {nameof(postId)}={postId} : Time span {span}"));
        }

        public RetryPolicy<Result<Post, HttpRequester.StatusCode>> GetPostPolicy(int postId)
        {
            var getPostPolicy = Policy<Result<Post, HttpRequester.StatusCode>>
                .HandleResult(r => !r.HasValue).WaitAndRetry(_botService.Config.Retry, PolynomiallyTimeSpan,
                (r, span)
                    => Log.Warning(
                        $"Retry Get Post {nameof(postId)}={postId} : Time span {span}"
                    ));
            return getPostPolicy;
        }

        public RetryPolicy<Result<UserInfoByUserId, HttpRequester.StatusCode>> GetUserPolicy(int userId)
        {
            var getPostPolicy = Policy<Result<UserInfoByUserId, HttpRequester.StatusCode>>
                .HandleResult(r => !r.HasValue).WaitAndRetry(_botService.Config.Retry, PolynomiallyTimeSpan,
                (r, span)
                    => Log.Warning(
                        $"Retry Get User {nameof(userId)}={userId} : Time span {span}"
                    ));
            return getPostPolicy;
        }

        public RetryPolicy<Result<Post[], HttpRequester.StatusCode>> GetUsersPostPolicy(int userId)
        {
            var getPostPolicy = Policy<Result<Post[], HttpRequester.StatusCode>>
                .HandleResult(r => !r.HasValue).WaitAndRetry(_botService.Config.Retry, PolynomiallyTimeSpan,
                (r, span)
                    => Log.Warning(
                        $"Retry Get User {nameof(userId)}={userId} : Time span {span}"
                    ));
            return getPostPolicy;
        }

        public RetryPolicy<HttpRequester.StatusCode> GetPostReplyPolicy(int postId, string commentId)
        {
            var getPostPolicy = Policy<HttpRequester.StatusCode>
                .HandleResult(p => p.IsPostError()).WaitAndRetry(_botService.Config.Retry, PolynomiallyTimeSpan,
                (r, span)
                    => Log.Warning(
                        $"Retry Post Reply {nameof(postId)}={postId} {nameof(commentId)}={commentId} : Time span {span}"
                    ));
            return getPostPolicy;
        }

        private static TimeSpan PolynomiallyTimeSpan(int retryAttempt)
        {
            return TimeSpan.FromSeconds(1);
            // return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)); //TODO Sometimes awaiting is too long  
        }
    }
}