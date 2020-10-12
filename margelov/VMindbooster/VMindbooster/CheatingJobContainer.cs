using System;
using System.Collections.Generic;

namespace VMindbooster
{
    public class CheatingJobContainer
    {
        private CheatingJobContainer()
        {
        }
        
        public static CheatingJobContainer Setup(ThresholdConfiguration configuration)
        {
            var container = new CheatingJobContainer();
            
            var vmindbookeClient = new VMindbookeClient();
            var usedContentRegistry = new UsedContentRegistry();;
            var commentLikedPostJob = new CommentLikedPost(configuration, usedContentRegistry, vmindbookeClient);
            container.Register(commentLikedPostJob);
            var replyLikedComment = new ReplyLikedComment(configuration, usedContentRegistry, vmindbookeClient);
            container.Register(replyLikedComment);

            return container;
        }

        public IReadOnlyCollection<ICheatingJob> Jobs => _cheatingJobs.Values;

        public ICheatingJob GetByType(Type type) => _cheatingJobs[type];
        
        private void Register<T>(T cheatingJob) where T : ICheatingJob
        {
            _cheatingJobs.Add(typeof(T), cheatingJob);
        }
        
        private readonly Dictionary<Type, ICheatingJob> _cheatingJobs = new Dictionary<Type, ICheatingJob>();
    }
}