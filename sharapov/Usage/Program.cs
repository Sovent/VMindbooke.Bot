using System;
using System.Data;
using System.Dynamic;
using Bogus.DataSets;
using Hangfire;
using Serilog;
using VMindbookeBot;

namespace Usage
{
    class Program
    {
        static void Main(string[] args)
        {
            var method = args[0];
            var compositionRoot = CompositionRoot.Create();
            try
            {
                Action(method, compositionRoot);
            }
            catch (Exception ex)
            {
                Log.Fatal("Something go wrong " + ex);
            }
        }

        private static void Action(string method, CompositionRoot compositionRoot)
        {
            switch (method)
            {
                case "write_comment":
                    compositionRoot.BotService.WriteComment(16);
                    break;
                case "write_reply":
                    compositionRoot.BotService.WriteReply(16);
                    break;
                case "copy_post_content":
                    compositionRoot.BotService.CopyPost(17);
                    break;
                case "copy_most_liked_post":
                    var hours24 = new TimeSpan(1, 0, 0, 0);
                    compositionRoot.BotService.CopyMostLikedPost(1216, hours24);
                    break;
                case "boost":
                    compositionRoot.BotService.Boost(1216, ()=>  Console.ReadKey());
                    break;
                
            }
        }
    }
}