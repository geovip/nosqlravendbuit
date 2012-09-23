using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Core.IService;

namespace UIT.NoSQL.Service
{
    public class TopicService : ITopicService 
    {
        private IDocumentSession session;

        public TopicService(IDocumentSession session)
        {
            this.session = session;
        }

        public List<TopicObject> GetAll()
        {
            return null;
        }

        public void Save(TopicObject topic)
        {
            session.Store(topic);
            session.SaveChanges();

        }
    }
}
