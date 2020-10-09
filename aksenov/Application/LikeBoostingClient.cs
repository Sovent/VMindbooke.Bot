using System;
using System.Collections.Generic;
using System.IO;
using Serilog;
using VMindbooke.Bot;
using VMindbooke.Bot.Application;
using VMindbooke.Bot.Domain;
using VMindbooke.Bot.Infrastructure;

namespace Application
{
    public class LikeBoostingClient
    {
        private static LikeBoostingService _boostingService;

        public LikeBoostingClient()
        {
            
        }

        static LikeBoostingClient()
        {
            BotSettings settings;
            try
            {
                settings = BotSettings.FromJsonFile("appsettings.json");
            }
            catch
            {
                Log.Error("Settings file 'appsettings.json' not found.");
                throw new FileNotFoundException("appsettings.json");
            }
            
            var apiRequestsService = new APIRequestsService(settings.ServerAddress, Log.Logger);
            var userService = new UserService(apiRequestsService);
            
            User boostedUser;
            if (userService.DoesUserExists(settings.UserId))
            {
                boostedUser = new User(settings.UserId, settings.UserToken, settings.UserName, new List<Like>());
            }
            else
            {
                boostedUser = userService.RegisterUser(settings.UserName);
                if (boostedUser == null || !boostedUser.IsValid())
                {
                    throw new ArgumentNullException("boostedUser");
                }

                Log.Information($"User [Id: {boostedUser.Id}, Token: {boostedUser.Token}] was created.");
            }

            _boostingService = new LikeBoostingService(settings, 
                apiRequestsService, 
                DateTime.Now,
                boostedUser,
                new SpamRepository(),
                new ProcessedObjectsRepository(), 
                Log.Logger);
        }

        public void CommentsWriting()
        {
            Log.Information("Comments writing scenario started.");
            _boostingService.CommentsWritingScenario();
            Log.Information("Comments writing scenario finished.");
        }
        
        public void RepliesWriting()
        {
            Log.Information("Replies writing scenario started.");
            _boostingService.RepliesWritingScenario();
            Log.Information("Replies writing scenario finished.");
        }
        
        public void PostsCopyingByLikes()
        {
            Log.Information("Posts copying by likes scenario started.");
            _boostingService.PostsCopyingByLikesScenario();
            Log.Information("Posts copying by likes scenario finished.");
        }
        
        public void PostsCopyingByUsers()
        {
            Log.Information("Posts copying by users scenario started.");
            _boostingService.PostsCopyingByUsersScenario();
            Log.Information("Posts copying by users scenario finished.");
        }

        public void BoostFinish()
        {
            Log.Information("Boost finish scenario started.");
            _boostingService.BoostFinishScenario();
            Log.Information("Boost finish scenario finished.");
        }
    }
}