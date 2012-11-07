using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;

namespace UIT.NoSQL.Core.IService
{
    public interface ITopicService
    {
        TopicObject Load(string id);
        List<TopicObject> GetAll();
        void Save(TopicObject topic);
    }
}
