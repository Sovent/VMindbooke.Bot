﻿using Microsoft.Extensions.Configuration;
using VmindbookeSDK;
using VmindbookeSDK.Entities;
using VMindbookeSDK.Entities;

namespace ShitBot
{
    public class ShitBotClient
    {
        
            private IVmindbookeClient _vMindbooke;
            private UserCredentials _credentials;
            private IMessageGenerator _messageGenerator;

            public ShitBotClient()
            {
                IConfiguration cfg = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                _vMindbooke = new VmindBookeClient(cfg["ApiBaseUrl"]);
                _credentials = new UserCredentials(int.Parse(cfg["UserId"]), cfg["UserToken"]);
                _messageGenerator = new CleverMessageGenerator();
            }

            public void Comment(Post post) =>
                _vMindbooke.CreateComment(
                    _credentials,
                    post.Id,
                    new NewComment(_messageGenerator.GetComment()));

            public void Reply(Post post, Comment comment) =>
                _vMindbooke.CreateReply(
                    _credentials,
                    post.Id,
                    comment.Id,
                    new NewComment(_messageGenerator.GetReply()));

            public void Repost(Post post) =>
                _vMindbooke.CreatePost(
                    _credentials,
                    _credentials.Id,
                    new NewPost(_messageGenerator.GetTitle(), post.Content));
        
    }
}