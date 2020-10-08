using System;
using System.Collections.Generic;
using System.Linq;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Infrastructure
{
    public class SpamRepository : ISpamRepository
    {
        private List<String> _postTitles = new List<String>()
        {
            "New interesting post.",
            "My precious post.",
            "My beautiful post.",
            "Let's talk about...",
            "You have to read this post"
        };
        
        private List<String> _comments = new List<String>()
        {
            "Hey there. Nice post!",
            "So interesting",
            "Great post theme",
            "Such a beautiful post.",
            "Such an interesting post."
        };
        
        private List<String> _replies = new List<String>()
        {
            "Hey there. Nice comment!",
            "So interesting",
            "Great comment!",
            "I think you are wrong",
            "Entertaining opinion"
        };

        public string GetRandomPostTitle()
        {
            var random = new Random();
            var index = random.Next(0, _postTitles.Count());
            return _postTitles[index];
        }
        
        public string GetRandomComment()
        {
            var random = new Random();
            var index = random.Next(0, _comments.Count());
            return _comments[index];
        }
        
        public string GetRandomReply()
        {
            var random = new Random();
            var index = random.Next(0, _replies.Count());
            return _replies[index];
        }
    }
}