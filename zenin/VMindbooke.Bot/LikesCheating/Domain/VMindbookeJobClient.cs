using VMindbooke.SDK;
using Hangfire;
using System.Collections.Generic;

namespace LikesCheating.Domain
{
    public class VMindbookeJobClient : IVMindbookeJobClient
    {
        public VMindbookeJobClient(VMindbookeClient client)
        {
            _client = client;
        }
        public void StartJobs(int userId, string token, Thresholds thresholds)
        {
            User targetUser = _client.GetUser(userId);
            IReadOnlyCollection<User> users = _client.GetUsers();
            IReadOnlyCollection<Post> posts = _client.GetPosts();
            
            //RecurringJob.AddOrUpdate<VMindbookeClient>(
                //client => posts = client.GetPosts(),
                //Cron.Minutely);
            
            foreach (var post in posts)
            {
                RecurringJob.AddOrUpdate<Cheating>(
                    cheating => cheating.CommentIfLikesMoreThanThreshold(
                        post, thresholds.PostThresholdToComment, token),
                    Cron.Minutely);
                foreach (var comment in post.Comments)
                {
                    
                }
            }
        }

        public void StopJobs()
        {
            
        }

        private readonly VMindbookeClient _client;
    }
}