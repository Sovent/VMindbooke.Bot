using System;
using System.Collections.Generic;
using Hangfire;
using Usage.Domain.Entities;

namespace Usage.Domain
{
    public class VmBoosterBot
    {
        public VmBoosterBot(int likeLimit,
            IPostCommenter postCommenter,
            ICommentReplier commentReplier,
            IPostsStealer postsStealer,
            ILikeLimitChecker likeLimitChecker)
        {
            _likeLimit = likeLimit;
            _postCommenter = postCommenter;
            _commentReplier = commentReplier;
            _postsStealer = postsStealer;
            _likeLimitChecker = likeLimitChecker;
        }

        private readonly IPostCommenter _postCommenter;
        private readonly ICommentReplier _commentReplier;
        private readonly IPostsStealer _postsStealer;
        private readonly ILikeLimitChecker _likeLimitChecker;

        private readonly int _likeLimit;
        private static Action _boosterJobs = () => { };
        private readonly HashSet<string> _jobIds = new HashSet<string>();
        public bool IsBoosting { get; private set; }
        
        public VmBoosterBot CommentPosts(
            int minNumberOfDailyLikesToCommentPost,
            CommentContent comment)
        {
            var jobId = Guid.NewGuid().ToString();
            _jobIds.Add(jobId);
            _boosterJobs += () => RecurringJob.AddOrUpdate(jobId,
                () => _postCommenter.CommentPosts(minNumberOfDailyLikesToCommentPost, comment), Cron.Minutely);
            return this;
        }

        public VmBoosterBot ReplyComments(
            int minNumberOfLikesToReplyToComment,
            CommentContent comment)
        { 
            var jobId = Guid.NewGuid().ToString();
            _jobIds.Add(jobId);
            _boosterJobs += () => RecurringJob.AddOrUpdate(jobId, () => _commentReplier.ReplyToComments(
                minNumberOfLikesToReplyToComment,
                comment), Cron.Minutely);
            return this;
        }
        
        public VmBoosterBot StealPosts(
            int likesThreshold, string newPostTitle)
        { 
            var jobId = Guid.NewGuid().ToString();
            _jobIds.Add(jobId);
            _boosterJobs += () => RecurringJob.AddOrUpdate(jobId, () => _postsStealer.StealPosts(
                    likesThreshold, newPostTitle),
                Cron.Minutely);;
            return this;
        }
        
        public VmBoosterBot StealBestPostOfMostLikedUsers(
            int likesThreshold)
        { 
            var jobId = Guid.NewGuid().ToString();
            _jobIds.Add(jobId);
            _boosterJobs += () =>
                RecurringJob.AddOrUpdate(jobId,
                    () => _postsStealer.StealTheBestPostOfMostLikedUser(likesThreshold),
                    Cron.Minutely);
            return this;
        }

        public void StopBoosting()
        {
            foreach (var jobId in _jobIds)
            {
                RecurringJob.RemoveIfExists(jobId);
                IsBoosting = false;
            }
            Console.WriteLine("STOPPED BOOST");
        }

        public void StartBoosting()
        {
            Console.WriteLine("STARTED BOOST");
            _boosterJobs.Invoke();
            RecurringJob.AddOrUpdate(() => CheckLike(),
                Cron.Minutely);
        }

        public void CheckLike()
        {
            if (_likeLimitChecker.IsLimitExceeded(_likeLimit))
                StopBoosting();
        }
    }
}