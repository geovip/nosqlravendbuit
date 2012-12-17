using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Core.Domain;
using Raven.Client;
using Raven.Abstractions.Data;
using Raven.Json.Linq;

namespace UIT.NoSQL.Service
{
    public class TopicService : ITopicService
    {
        private IDocumentSession session;
        
        public TopicService(IDocumentSession session)
        {
            this.session = session;
        }

        public TopicObject Load(string id)
        {
            return session.Load<TopicObject>(id);
        }

        public List<TopicObject> GetAll()
        {
            return session.Query<TopicObject>().ToList();
        }

        public List<TopicObject> GetByGroup()
        {
            return null;
        }

        public void Save(TopicObject topic)
        {
            session.Store(topic);
            session.SaveChanges();
        }

        public void UpdateNumberOfCommentInDenormalizedTopic(string groupId, string topicId)
        {
            session.Advanced.DatabaseCommands.Patch(
                groupId,
                new[]
                     {
                         new PatchRequest
                             {
                                 Type = PatchCommandType.Modify, 
                                 Name = "ListTopic", 
                                 Nested = new[]
                                    {
                                        new PatchRequest
                                            {
                                                Type = PatchCommandType.Set, 
                                                Name = "NumberOfComment", 
                                                Value = new RavenJValue(1)
                                            }
                                    }
                             }
                     }
                     );
        }
    }
}
