using System;
using System.Dynamic;
using VMindbookeBot;

namespace Usage
{
    class Program
    {
        static void Main(string[] args)
        {
            var method = args[0];
            var compositionRoot = CompositionRoot.Create();


            switch (method)
            {
                case "write_comment":
                    compositionRoot.BotService.WriteComment(16);
                    break;
                case "write_reply":
                    compositionRoot.BotService.WriteReply(16);
                    break;
                // case "copy_post_content":
                //     compositionRoot.BotService.Reply();
                //     break;
                // case "copy_most_liked_post":
                //     compositionRoot.BotService.Reply();
                //     break;
                // case "stop_boost":
                //     compositionRoot.BotService.Reply();
                //     break;
                //
            }
        }
    }
}