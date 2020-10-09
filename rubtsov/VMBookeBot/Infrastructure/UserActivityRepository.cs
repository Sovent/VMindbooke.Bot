using System;
using System.Collections.Generic;
using VMBookeBot.Domain;

namespace VMBookeBot.Infrastructure
{
    public class UserActivityRepository : IUserActivityRepository
    {
        public List<int> AlreadyStolenPosts { get; set; }
        public List<Guid> AlreadyRepliedComments { get; set; }
        public List<int> AlreadyCommentedPosts { get; set; }
        public UserActivityRepository(List<int> alreadyStolenPosts, List<Guid> alreadyRepliedComments, List<int> alreadyCommentedPosts)
        {
            AlreadyStolenPosts = alreadyStolenPosts;
            AlreadyRepliedComments = alreadyRepliedComments;
            AlreadyCommentedPosts = alreadyCommentedPosts;
        }
        
    }
}