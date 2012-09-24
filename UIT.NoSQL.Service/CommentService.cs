using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Core.IService;
using Raven.Client;

namespace UIT.NoSQL.Service
{
    public class CommentService : ICommentService
    {
        private IDocumentSession session;

        public CommentService(IDocumentSession session)
        {
            this.session = session;
        }

        //Load Comment based on Id
        public CommentObject Load(string id)
        {
            return session.Load<CommentObject>(id);
        }

        // Get all comments
        public List<CommentObject> GetAll()
        {
            var comments = session.Query<CommentObject>();
            return comments.ToList();
        }

        public void Save(CommentObject comment)
        {
            session.Store(comment);
            session.SaveChanges();
        }

        //Delete a comment
        public void Delete(string id)
        {
            var comment = Load(id);
            session.Delete<CommentObject>(comment);
            session.SaveChanges();
        }
    }
}
