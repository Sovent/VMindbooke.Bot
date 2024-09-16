using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Hangfire;
using Serilog;

namespace VMindbookeBot
{
    public class BoostJob
    {
        public BoostJob()
        {
        }

        public void StartDailyBoost(int userId, int threshold, BotService context)
        {
            var maxUserId = context.Config.UserIdMaxValue;
            var runningBoostJobIds = StartBoostJob(maxUserId);
            var jobListId = CreateJobIds(runningBoostJobIds, out var checkLikeThresholdJobId);
            CheckAndStopIfLikesEnough(userId, threshold, checkLikeThresholdJobId, jobListId, context);
        }

        private static List<string> CreateJobIds(IEnumerable<string> runningBoostJobIds,
            out string checkLikeThresholdJobId)
        {
            var jobListId = new List<string>(runningBoostJobIds);
            checkLikeThresholdJobId = Guid.NewGuid().ToString();
            jobListId.Add(checkLikeThresholdJobId);
            return jobListId;
        }

        public static void CheckAndStopIfLikesEnough(int userId, int threshold, string checkLikeThresholdJobId,
            IList<string> jobListId, BotService context)
        {
            RecurringJob.AddOrUpdate<BoostJob>(checkLikeThresholdJobId,
                client => client.CheckLikeThreshold(userId, threshold, jobListId, context),
                CronSecondDelay());
        }

        private static Func<string> CronSecondDelay(int delay = 3)
        {
            return () => $"*/{delay} * * * * *";
        }

        public void CheckLikeThreshold(int userId, int threshold, IList<string> runningBoostJobIds, BotService context)
        {
            var pastTime = DateTime.Now - new TimeSpan(1, 0, 0, 0);
            var user = context.GetUser(userId);
            var userLikes = user.OptionalResult.Map(p => p.likes);
            var likesForPastTimeToNow = userLikes.Map(likes =>
            {
                return likes.Count(like => like.placingDateUtc > pastTime);
            });
            likesForPastTimeToNow.Do(likes =>
            {
                if (likes >= threshold)
                {
                    Log.Verbose($"Boost Stooped: likes >= threshold. likes={likes} threshold={threshold}");
                    StopJobs(runningBoostJobIds);
                }
            });
        }

        public static IEnumerable<string> StartBoostJob(int maxUserId)
        {
            Log.Information($"Create boost job");
            var ids = new List<string>();
            string id;
            id = AddJob<BotService>(client => client.WriteComment(GetRandomUserId(maxUserId)));
            ids.Add(id);
            id = AddJob<BotService>(client => client.WriteReply(GetRandomUserId(maxUserId)));
            ids.Add(id);
            id = AddJob<BotService>(client => client.CopyPost(GetRandomUserId(maxUserId)));
            ids.Add(id);
            id = AddJob<BotService>(client =>
                client.CopyMostLikedPost(GetRandomUserId(maxUserId), new TimeSpan(1, 0, 0, 0)));
            ids.Add(id);
            return ids;
        }

        public static string AddJob<T>(Expression<Action<T>> exp)
        {
            var guidJob = Guid.NewGuid().ToString();
            RecurringJob.AddOrUpdate<T>(guidJob, exp, CronSecondDelay());
            return Guid.NewGuid().ToString();
        }

        //poor implementation. Probably I should find maxUserId with using API
        private static int GetRandomUserId(int maxUserId)
        {
            return new Random().Next(maxUserId) + 1;
        }

        private static void StopJobs(IEnumerable<string> ids)
        {
            Log.Information("Boost Stooped: likes >= threshold");
            foreach (var id in ids)
            {
                RecurringJob.RemoveIfExists(id);
            }
        }
    }
}