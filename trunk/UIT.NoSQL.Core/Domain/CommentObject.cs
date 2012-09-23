using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIT.NoSQL.Core.Domain
{
    public class CommentObject
    {
        public string CommentID { get; set; }
        public UserObject User { get; set; }
        public TopicObject Topic { get; set; }
        public string Content { get; set; }
        public CommentObject ParentComment { get; set; }
    }
}