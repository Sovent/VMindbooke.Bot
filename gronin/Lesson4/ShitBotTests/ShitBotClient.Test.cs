using System;
using System.Linq;
using NUnit.Framework;
using ShitBot;
using VmindbookeSDK;

namespace ShitBotTests
{
    public class ShitBotClientTests
    {
        [Test]
        public void Comment_commentSent()
        {
            var generator = new CleverMessageGenerator();
            var bot = new ShitBotClient(generator);
            var client = new  VmindBookeClient("http://135.181.101.47");

            var testpost = client.GetPost(10);
            bot.Comment(testpost);
            testpost = client.GetPost(10);
            var lastComment = testpost.Comments.Last();

            Assert.AreEqual(lastComment.Content,generator.GetComment());
            Assert.AreEqual(lastComment.PostingDateUtc.Minute,DateTime.Now.Minute);
        }
        
        [Test]
        public void Repost_RepostMade()
        {
            var generator = new CleverMessageGenerator();
            var bot = new ShitBotClient(generator);
            var client = new  VmindBookeClient("http://135.181.101.47");

            var testpost = client.GetPost(10);
            bot.Repost(testpost);
            var repost = client.GetUserPosts(2130).Last();

            Assert.AreEqual(repost.Title,generator.GetTitle());
            Assert.AreEqual(repost.PostingDateUtc.Minute,DateTime.Now.Minute);
        }
        
        [Test]
        public void Reply_ReplySent()
        {
            var generator = new CleverMessageGenerator();
            var bot = new ShitBotClient(generator);
            var client = new  VmindBookeClient("http://135.181.101.47");

            var testPost = client.GetPost(10);
            var testComment = testPost.Comments.First();
            
            bot.Reply(testPost,testComment);
            
            testPost = client.GetPost(10);
            testComment = testPost.Comments.First();
            
            Assert.Pass();
            Assert.AreEqual(testComment.Replies.Last().Content,generator.GetReply());
            
        }
    }
}