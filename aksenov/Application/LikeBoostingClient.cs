using System;
using System.IO;
using Serilog;
using VMindbooke.Bot;
using VMindbooke.Bot.Application;
using VMindbooke.Bot.Infrastructure;

namespace Application
{
    public class LikeBoostingClient
    {
        private LikeBoostingService _service;

        public LikeBoostingClient()
        {
            BotSettings settings;
            try
            {
                settings = BotSettings.FromJsonFile("appsettings.json");
            }
            catch (Exception e)
            {
                Log.Error("Settings file 'appsettings.json' not found.");
                throw new FileNotFoundException("appsettings.json");
            }

            var apiRequestsService = new APIRequestsService(settings, Log.Logger);
            
            _service = new LikeBoostingService(settings, 
                apiRequestsService, 
                DateTime.Now,
                new SpamRepository(), 
                new HashesRepository(), 
                new ProcessedObjectsRepository(), 
                Log.Logger);
        }

        public void CommentsWriting()
        {
            Log.Information("Comments writing scenario started.");
            _service.CommentsWritingScenario();
            Log.Information("Comments writing scenario finished.");
        }
        
        public void RepliesWriting()
        {
            Log.Information("Replies writing scenario started.");
            _service.RepliesWritingScenario();
            Log.Information("Replies writing scenario finished.");
        }
        
        public void PostsCopyingByLikes()
        {
            Log.Information("Posts copying by likes scenario started.");
            _service.PostsCopyingByLikesScenario();
            Log.Information("Posts copying by likes scenario finished.");
        }
        
        public void PostsCopyingByUsers()
        {
            Log.Information("Posts copying by users scenario started.");
            _service.PostsCopyingByUsersScenario();
            Log.Information("Posts copying by users scenario finished.");
        }

        public void BoostFinish()
        {
            Log.Information("Boost finish scenario started.");
            _service.BoostFinishScenario();
            Log.Information("Boost finish scenario finished.");
        }
    }
}