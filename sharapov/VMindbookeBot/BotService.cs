using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Hangfire;
using Hangfire.MemoryStorage;
using Polly;
using Polly.Retry;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using VMindbookeBot;

namespace Usage
{
    public class BotService
    {
        private readonly Configuration _config;
        private readonly Bot _bot;
        private readonly IWordsGenerator _wordsGenerator;

        private BotService(Configuration config, Bot bot, IWordsGenerator wordsGenerator)
        {
            _config = config;
            _bot = bot;
            _wordsGenerator = wordsGenerator;
        }

        public static BotService Create(Configuration config)
        {
            CreateLogger(config);
            return new BotService(config, new Bot(config.Token), WordsGenerator.Create(config.Local));
        }

        private static void CreateLogger(Configuration config)
        {
            var configuration = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(config.LogName)
                .WriteTo.Console();
            Log.Logger = configuration.CreateLogger();
        }

        public void WriteComment(int postId)
        {
            var threshold = _config.WriteCommentThreshold;
            var likesFromPost = GetLikesPost(postId);
            var likesCount = likesFromPost.OptionalResult.Map(x => x.Count());
            likesCount.Do(likes =>
                {
                    if (likes > threshold)
                    {
                        PostComment(postId);
                    }
                }
            );
        }

        public void WriteReply(int commentId)
        {
            var threshold = _config.ReplyCommentThreshold;
            var likesFromReply = GetLikesComment(commentId);
            var likesCount = likesFromReply.OptionalResult.Map(x => x.Count());
            likesCount.Do(likes =>
                {
                    if (likes > threshold)
                    {
                        PostReply(commentId);
                    }
                }
            );
        }

    private void PostComment(int postId)
        {
            var writeCommentPolicy = Policy<Bot.StatusCode>
                .HandleResult(p => p.IsWriteError())
                .WaitAndRetry(_config.Retry, PolynomiallyTimeSpan,
                    (r, span) =>
                        Log.Warning(
                            $"WriteComment scenario : Retry WriteComment to {nameof(postId)}={postId} : Attempt {span}"));
            var text = _wordsGenerator.GetText();
            writeCommentPolicy.Execute(() => _bot.WriteComment(postId, _wordsGenerator.GetText()));
            Log.Information($"Comment to {nameof(postId)}={postId} posted");
            Log.Verbose($"{nameof(text)}={text}");
        }

        private Result<IEnumerable<Like>, Bot.StatusCode> GetLikesPost(int postId)
        {
            var getLikesPolicy = Policy<Result<IEnumerable<Like>, Bot.StatusCode>>
                .HandleResult(r => !r.HasValue)
                .WaitAndRetry(_config.Retry, PolynomiallyTimeSpan,
                    (r, span)
                        => Log.Warning(
                            $"WriteComment scenario : Retry GetLikesByPostId to {nameof(postId)}={postId} : Attempt {span}"
                        ));
            return getLikesPolicy.Execute(() => _bot.GetLikesByPostId(postId));
        }
        
        private Result<IEnumerable<Like>, Bot.StatusCode> GetLikesComment(int postId)
        {
            var getLikesPolicy = Policy<Result<IEnumerable<Like>, Bot.StatusCode>>
                .HandleResult(r => !r.HasValue)
                .WaitAndRetry(_config.Retry, PolynomiallyTimeSpan,
                    (r, span)
                        => Log.Warning(
                            $"WriteComment scenario : Retry GetLikesByPostId to {nameof(postId)}={postId} : Attempt {span}"
                        ));
            return getLikesPolicy.Execute(() => _bot.GetLikesByPostId(postId));
        }

        private static TimeSpan PolynomiallyTimeSpan(int retryAttempt)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
        }
    }
}