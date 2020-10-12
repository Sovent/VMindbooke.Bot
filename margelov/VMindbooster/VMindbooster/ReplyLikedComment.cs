using System;

namespace VMindbooster
{
    public class ReplyLikedComment : ICheatingJob
    {
        private readonly ThresholdConfiguration _configuration;
        private readonly IUsedContentRegistry _usedContentRegistry;
        private readonly IVMindbookeClient _vMindbookeClient;

        public ReplyLikedComment(
            ThresholdConfiguration configuration,
            IUsedContentRegistry usedContentRegistry,
            IVMindbookeClient vMindbookeClient)
        {
            _configuration = configuration;
            _usedContentRegistry = usedContentRegistry;
            _vMindbookeClient = vMindbookeClient;
        }
        
        public void Execute()
        {
            // all logic
            
            Console.WriteLine("Reply liked comment executed");
        }
    }
}