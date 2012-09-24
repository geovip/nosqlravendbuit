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

        //Load Topic based on Id
        public TopicObject Load(string id)
        {
            return session.Load<TopicObject>(id);
        }

        // Get all topics
        public List<TopicObject> GetAll()
        {
            var topics = session.Query<TopicObject>();
            return topics.ToList();
        }

        public void Save(TopicObject topic)
        {
            session.Store(topic);
            session.SaveChanges();
        }

        //Delete a topic
        public void Delete(string id)
        {
            var topic = Load(id);
            session.Delete<TopicObject>(topic);
            session.SaveChanges();
        }
    }
}
