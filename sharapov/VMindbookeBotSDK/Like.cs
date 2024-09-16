using System;
using System.Collections.Generic;

namespace VMindbookeBot
{
    public class Like
    {
        public string id { get; set; }
        public int authorId { get; set; }
        public DateTime placingDateUtc { get; set; }
    }

    public class Post
    {
        public int id { get; set; }
        public int authorId { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public DateTime postingDateUtc { get; set; }
        public Comment[] comments { get; set; }
        public Like[] likes { get; set; }
    }

    public class Comment
    {
        public string id { get; set; }
        public int authorId { get; set; }
        public string content { get; set; } 
        public DateTime postingDateUtc { get; set; } 
        public Reply[] replies { get; set; } 
        public Like[] likes { get; set; } 
    }

    public class Reply
    {
        public string id { get; set; }
        public int authorId { get; set; }
        public string content { get; set; }
        public DateTime postingDateUtc { get; set; }
        public Reply[] replies { get; set; }
        public Like[] likes { get; set; }
    }
    
    public class UserInfoByUserId
    {
        public int id { get; set; }
        public string name { get; set; }
        public Like[] likes { get; set; }
  }

}