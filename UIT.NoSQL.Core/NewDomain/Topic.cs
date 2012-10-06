using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.NewDomain
{
    public class Topic
    {
        public string TopicId { get; set; }
        public string TopicName { get; set; }
        public string Content { get; set; }
        public DenormalizedUser CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModified { get; set; }
        public UInt32 NumberOfView { get; set; }
        public UInt32 NumberOfComment { get; set; }

        public List<Comment> ListComment { get; set; }

        public Topic()
        {
            ListComment = new List<Comment>();
        }

        public UInt32 GetNumberOfComment()
        { 
            return (UInt32)ListComment.Count();
        }

    }

    public class DenormalizedTopic
    {
        public string TopicId { get; set; }
        public string TopicName { get; set; }
        public DenormalizedUser CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModified { get; set; }
        public UInt32 NumberOfView { get; set; }
        public UInt32 NumberOfComment { get; set; }
    }

    public class Comment
    {
        public string CommentId { get; set; }
        public string Content { get; set; }
        public DenormalizedUser CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
