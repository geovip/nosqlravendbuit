using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;

namespace UIT.NoSQL.Core.IService
{
    public interface ICommentService
    {
        CommentObject Load(string id);
        List<CommentObject> GetAll();
        void Save(CommentObject comment);
        void Delete(string id);
    }
}
