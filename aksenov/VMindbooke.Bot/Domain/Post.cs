﻿using System;

namespace VMindbooke.Bot.Domain
{
    public class Post: IValidObject
    {
        public Post(int id, int authorId, string title, string content, DateTime postingDateUtc, Comment[] comments, Like[] likes)
        {
            Id = id;
            AuthorId = authorId;
            Title = title;
            Content = content;
            PostingDateUtc = postingDateUtc;
            Comments = comments;
            Likes = likes;
        }
        
        public int Id { get; }

        public int AuthorId { get; }

        public string Title { get; }

        public string Content { get; }

        public DateTime PostingDateUtc { get; }

        public Comment[] Comments { get; }

        public Like[] Likes { get; }

        public override int GetHashCode()
        {
            return (Id ^ AuthorId + Title.GetHashCode() + Content.GetHashCode()) 
                   ^ (PostingDateUtc.GetHashCode() + Comments.Length * Likes.Length);
        }

        public bool IsValid()
        {
            if (Title == null || Content == null || Comments == null || Likes == null)
                return false;

            return true;
        }
    }
}