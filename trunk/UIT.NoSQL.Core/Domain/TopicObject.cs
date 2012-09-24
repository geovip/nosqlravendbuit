using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIT.NoSQL.Core.Domain
{
    public class TopicObject
    {
        public string TopicID { get; set; }
        public UserObject User { get; set; }
        public GroupObject Group { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public int NumView { get; set; }
        public bool IsDetele { get; set; }

        public List<CommentObject> ListComment { get; set; }

        public TopicObject()
        {
            ListComment = new List<CommentObject>();
        }
    }
}