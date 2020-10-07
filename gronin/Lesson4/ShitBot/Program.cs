using System;
using System.Linq;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using VmindbookeSDK;

namespace ShitBot
{
    class Program
    {
        static void Main(string[] args)
        {
           /* var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            
            
            var client = new VmindBookeClient();
            var users = client.GetUsers();
            foreach (var i in users)
            {
                Console.WriteLine(i.Name);
            }

            var newUserser = client.RegisterUser(new User("asdfgh")); 
            Console.WriteLine(newUserser);

            var post = client.AddPost(new Post("dsfsf", "dsfsfsf"),63);
            Console.WriteLine(post);*/
           /*GlobalConfiguration.Configuration.UseMemoryStorage();
           RecurringJob.AddOrUpdate(()=>Console.WriteLine("job done"),Cron.Minutely);
           BackgroundJob.Schedule(() => Console.WriteLine("another one"), TimeSpan.FromSeconds(30));
           
           var client = new VmindBookeClient();

           RecurringJob.AddOrUpdate(() =>
           
               Console.WriteLine(client.AddPost(new Post("ad","da"),63 ))
           , Cron.Minutely);
           
           
           RecurringJob.AddOrUpdate(() =>
           
                   Console.WriteLine(client.LikeAll(63))
               , Cron.Minutely);
           
           using (var backgroundServer = new BackgroundJobServer())
           {
               Console.WriteLine("started");
               
               Console.ReadKey(); 
           }*/


           var logger = new LoggerConfiguration()
               .WriteTo.File("reg.log", restrictedToMinimumLevel: LogEventLevel.Information)
               .WriteTo.Console()
               .CreateLogger();
           logger.Error("dfghjk");
           
           Console.WriteLine("keks");

        }
    }
}
