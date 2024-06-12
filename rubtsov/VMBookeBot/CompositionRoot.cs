using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using VMBook.SDK;
using VMBookeBot.Domain;
using VMBookeBot.Infrastructure;

namespace VMBookeBot
{
    public class CompositionRoot
    {
        public BotThiefService BotThiefService { get; }
        public int CurrentUserId { get; }
            
        public CompositionRoot()
        {
            var logFilePath =
                Directory.GetParent(Environment.CurrentDirectory).Parent?.Parent?.Parent?.FullName + "\\log-1.txt";

            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Logger = logger;

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var baseUrl = configuration["serverUrl"];
            CurrentUserId = int.Parse(configuration["currentUserId"]);
            var currentUserToken = configuration["authorizationToken"];
            var vmbClient = new VmbClient(baseUrl, CurrentUserId, currentUserToken);
            
            var jobRetryer = new RequestsRetryer(vmbClient);
            var collectionsFilter = new RequestsFilter(vmbClient);

            var repositoryLoader = new RepositoryLoader();
            var vmBookJobClient = new BotThief(jobRetryer, collectionsFilter, RepositoryLoader.Load());
            BotThiefService = new BotThiefService(vmBookJobClient, collectionsFilter, configuration);
        }
    }
}