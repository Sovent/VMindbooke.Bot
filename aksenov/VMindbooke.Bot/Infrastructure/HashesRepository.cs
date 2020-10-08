using System.Collections.Generic;
using System.Linq;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Infrastructure
{
    public class HashesRepository: IHashesRepository
    {
        private readonly Dictionary<int, int> _postsHashes = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _usersHashes = new Dictionary<int, int>();

        public bool DoesContainPostHashWith(int postId)
        {
            return _postsHashes.ContainsKey(postId);
        }

        public bool DoesContainUserHashWith(int userId)
        {
            return _usersHashes.ContainsKey(userId);
        }

        public int GetPostHashBy(int postId)
        {
            if (_postsHashes.ContainsKey(postId))
                return _postsHashes[postId];
            
            throw new KeyNotFoundException();
        }

        public int GetUserHashBy(int userId)
        {
            if (_usersHashes.ContainsKey(userId))
                return _usersHashes[userId];
            
            throw new KeyNotFoundException();
        }

        public void AddOrUpdatePostHash(int postId, int postHash)
        {
            _postsHashes[postId] = postHash;
        }

        public void AddOrUpdateUserHash(int userId, int userHash)
        {
            _usersHashes[userId] = userHash;
        }
    }
}