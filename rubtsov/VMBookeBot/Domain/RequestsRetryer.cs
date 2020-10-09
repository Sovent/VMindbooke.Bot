using System;
using System.Net;
using Polly;
using Polly.Retry;
using RestSharp;
using Serilog;
using VMBook.SDK;

namespace VMBookeBot.Domain
{
    public class RequestsRetryer : IRequestRetryer
    {
        private readonly IVmbClientRetryer _vmbClient;
        public RequestsRetryer(IVmbClientRetryer vmbClient)
        {
            _vmbClient = vmbClient;
        }
        
        public bool TryWriteComment(int postId, int retryCount = 5)
        {
            Log.Information($"Trying to write new comment under the post {postId}");
            var clientRequestResult = RetryPolicy(retryCount)
                .ExecuteAndCapture(() => _vmbClient.WriteComment(postId)).Outcome;
            var successLogMessage = $"Comment was written under the post {postId}";
            var failingLogMessage = $"Unable to write comment under the post {postId}";
            LogRequestResult(clientRequestResult, successLogMessage, failingLogMessage);

            return clientRequestResult == OutcomeType.Successful;
        }

        private static RetryPolicy<IRestResponse> RetryPolicy(int retryCount)
        {
            var retryPolicy =
                Policy<IRestResponse>
                    .HandleResult(response => response.StatusCode == HttpStatusCode.InternalServerError)
                    .Retry(retryCount, (response, retryNumber) =>
                    {
                        Log.Information($"Attempt number: {retryNumber}");
                        Log.Error($"{response.Result.StatusDescription}");
                    });
            return retryPolicy;
        }

        private static void LogRequestResult(OutcomeType result, string successLogMessage, string failingLogMessage)
        {
            if (result == OutcomeType.Successful)
            {
                Log.Information(successLogMessage);
            }
            else
            {
                Log.Warning(failingLogMessage);
            }
        }

        public bool TryWriteReplyUnderComment(int postId, Guid commentId, int retryCount = 5)
        {
            Log.Information($"Trying to write reply on comment id:{commentId} under the post {postId}");
            var clientRequestResult = RetryPolicy(retryCount)
                .ExecuteAndCapture(() => _vmbClient.WriteReplyUnderComment(postId, commentId)).Outcome;
            var successLogMessage = $"Reply to comment {commentId} was written under the post {postId}";
            var failingLogMessage = $"Unable to write reply to comment {commentId} under the post {postId}";
            LogRequestResult(clientRequestResult, successLogMessage, failingLogMessage);
            return clientRequestResult == OutcomeType.Successful;
        }
        
        public bool TryMakeCopyOfPost(string postTitle, string postContent, int originalPostId, int retryCount = 5)
        {
            Log.Information($"Trying to create new post from post with id: {originalPostId}");
            var clientRequestResult = RetryPolicy(retryCount)
                .ExecuteAndCapture(() => _vmbClient.MakeCopyOfPost(postTitle, postContent));
            var successLogMessage = $"New post id: {clientRequestResult.Result.Content} was created";
            var failingLogMessage = $"Unable to create new post from post {originalPostId}";
            LogRequestResult(clientRequestResult.Outcome, successLogMessage, failingLogMessage);
            
            return clientRequestResult.Outcome == OutcomeType.Successful;
        }
    }
}