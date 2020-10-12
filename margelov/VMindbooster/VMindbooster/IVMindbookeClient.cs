using System;

namespace VMindbooster
{
    public interface IVMindbookeClient
    {
        void Comment(int postId);

        void Reply(int postId, Guid commentId);
    }
}