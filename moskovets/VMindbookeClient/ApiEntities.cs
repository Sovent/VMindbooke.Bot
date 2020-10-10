using System;
using System.Collections.Generic;

namespace VMindbookeClient
{
    public class Like
    {
        public Like(string id, int authorId, DateTime postingDateUtc)
        {
            Id = id;
            AuthorId = authorId;
            PostingDateUtc = postingDateUtc;
        }

        public string Id { get; }
        public int AuthorId { get; }
        public DateTime PostingDateUtc { get; }
    }
    
    public class User
    {
        public User(string name, int id, List<Like> likes)
        {
            Name = name;
            Id = id;
            Likes = likes;
        }

        public string Name { get; }
        public int Id { get;  }
        public List<Like> Likes { get;  }
    }

    public class UserAuthInfo
    {
        public UserAuthInfo(string name, int id, string token)
        {
            Name = name;
            Id = id;
            Token = token;
        }

        public string Name { get; }
        public int Id { get;  }
        public string Token { get;  }
    }

    public class Comment
    {
        public Comment(string id, int authorId, string content, DateTime postingDateUtc, Like[] likes, Reply[] replies)
        {
            Id = id;
            AuthorId = authorId;
            Content = content;
            PostingDateUtc = postingDateUtc;
            Likes = likes;
            Replies = replies;
        }

        public string Id { get; }
        public int AuthorId { get; }
        public string Content { get; }
        public DateTime PostingDateUtc { get; }
        public Like[] Likes { get; }
        public Reply[] Replies { get; }
    }

    public class Reply : Comment
    {
        public Reply(string id, int authorId, string content, DateTime postingDateUtc, Like[] likes, Reply[] replies) 
            : base(id, authorId, content, postingDateUtc, likes, replies)
        {
        }
    }
    public class Post
    {
        public Post(int id, int authorId, string title, string content, DateTime postingDateUtc, Like[] likes, Comment[] comments)
        {
            Id = id;
            AuthorId = authorId;
            Title = title;
            Content = content;
            PostingDateUtc = postingDateUtc;
            Likes = likes;
            Comments = comments;
        }

        public int Id { get; }
        public int AuthorId { get; }
        public string Title { get; }
        public string Content { get; }
        public DateTime PostingDateUtc { get; }
        public Like[] Likes { get; }
        public Comment[] Comments { get; }
    }
}