using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Client.Document;

namespace UIT.NoSQL.Service
{
    public class Base
    {
        private static IDocumentStore documentStore;
        private IDocumentSession session;

        public Base()
        {
            documentStore = new DocumentStore { Url = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectRavenDB"].ConnectionString };
            documentStore.Initialize();
        }

        public IDocumentSession Session
        {
            get
            {
                if (session == null)
                {
                    session = documentStore.OpenSession();
                }
                return session;
            }
        }
    }
}
