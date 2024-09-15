using System;
using LikesCheating.Domain;
using LikesCheating.Application;

namespace CompositionRoot
{
    public class CompositionRoot
    {
        public CompositionRoot Create()
        {
            var jobClient = new VMindbookeJobClient();
            var cheatService = new CheatService(jobClient);
            return new CompositionRoot()
            {
                CheatService = cheatService
            };
        }

        public ICheatService CheatService { get; private set; }
    }
}