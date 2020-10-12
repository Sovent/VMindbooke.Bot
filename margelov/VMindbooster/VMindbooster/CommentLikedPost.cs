using System;

namespace VMindbooster
{
    public class CommentLikedPost : ICheatingJob
    {
        private readonly ThresholdConfiguration _configuration;
        private readonly IUsedContentRegistry _usedContentRegistry;
        private readonly IVMindbookeClient _vMindbookeClient;

        public CommentLikedPost(
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
            // this is logic
            
            Console.WriteLine("Comment liked post executed");
        }
    }
}