using System;
using System.Linq;
using NUnit.Framework;
using VMindbooke.SDK;

namespace VMindbooke.Bot.Tests
{
    public class VMindbookeClientUnitTests
    {
        [Test]
        public void GetUser_IdOfFirstUserFromVMindbooke()
        {
            var client = new VMindbookeClient("http://135.181.101.47");
            
            var user = client.GetUser(1);
            
            Assert.AreEqual(1, user.Id);
        }
        [Test]
        public void GetPost_IdOfFirstPostFromVMindbooke()
        {
            var client = new VMindbookeClient("http://135.181.101.47");
            
            var post = client.GetPost(1);
            
            Assert.AreEqual(1, post.Id);
        }
        [Test]
        public void TokenOfMyAccount_Post_ContentFromPostedPost()
        {
            var client = new VMindbookeClient("http://135.181.101.47");
            var myUserId = 54;
            var myToken = "b37bb83c3cc94bc6a4c12a707ed9907d";
            var title = "Just another shit post";
            var content = "Sorry, that's for test";
            var postContent = new PostContent(title, content);
            
            var postId = client.Post(postContent, myUserId, myToken);
            var post = client.GetPost(postId);
            
            Assert.AreEqual(title, post.Title);
            Assert.AreEqual(content, post.Content);
        }
        [Test]
        public void TokenOfMyAccount_Comment_ContentOfLastCommentFromFirstPost()
        {
            var client = new VMindbookeClient("http://135.181.101.47");
            var myToken = "b37bb83c3cc94bc6a4c12a707ed9907d";
            var content = "Shit comment for test";
            var commentContent = new CommentContent(content);
            var postId = 1;
            
            client.Comment(commentContent, postId, myToken);
            var post = client.GetPost(postId);
            var comment = post.Comments.ElementAt(post.Comments.Count-1);
            
            Assert.AreEqual(content, comment.Content);
        }
        [Test]
        public void TokenOfMyAccount_Reply_ContentOfLastReplyOfFirstCommentFromFirstPost()
        {
            var client = new VMindbookeClient("http://135.181.101.47");
            var myToken = "b37bb83c3cc94bc6a4c12a707ed9907d";
            var content = "Shit reply on shit comment for test";
            var replyContent = new ReplyContent(content);
            var postId = 1;
            var commentId = Guid.Parse("08aa6cc0-174a-47eb-9553-2fefd4fea943");
            
            client.Reply(replyContent, postId, commentId, myToken);
            var post = client.GetPost(postId);
            var comment = post.Comments.FirstOrDefault(comment => comment.Id == commentId);
            var reply = comment.Replies.ElementAt(comment.Replies.Count-1);
            
            Assert.AreEqual(content, reply.Content);
        }
    }
}