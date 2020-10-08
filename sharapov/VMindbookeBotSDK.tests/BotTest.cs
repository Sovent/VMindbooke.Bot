using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
using NUnit.Framework;
using VMindbookeBot;

namespace VMindbookeBotSDK.tests
{
    public class BotTest
    {
        [Test]
        public void NotTestBotCreation()
        {
            //arrange

            //act
            var bot = Bot.Create("TestUserName");

            //assert
        }

        [Test]
        public void NotTest1()
        {
            var bot = new Bot( "31d026e268e640c48963bef6d07785ed");
            // var likes = bot.GetPostLikes(14);
            // var code = bot.WriteComment(16, "Title From Ide", "Content From Ide");
            // var code = bot.WriteReply(16, "c2bda9c5-d20f-4ce5-8dfa-e518f9d3ae8c", "Reply From Ide");
            var code = bot.WritePost(16, "c2bda9c5-d20f-4ce5-8dfa-e518f9d3ae8c", "Reply From Ide");
            
        }
    }
}