using System;
using NUnit.Framework;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.UnitTests
{
    public class CommentTests
    {
        [Test]
        public void IsValid_NotValidCommentObjectInInput()
        {
            //arrange
            var comment = new Comment("id", 1, "content", DateTime.Now, null, new Like[0]);
            
            //act
            var isValid = comment.IsValid();

            //assert
            Assert.IsFalse(isValid);
        }
    }
}