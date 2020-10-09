using System;
using System.Collections.Generic;

namespace VMBookeBot.Domain
{
    public interface IUserActivityRepository
    {
        List<int> AlreadyStolenPosts { get; set; }
        List<Guid> AlreadyRepliedComments { get; set; }
        List<int> AlreadyCommentedPosts { get; set; }
    }
}