using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Raven.Client;
using Raven.Client.Document;
using Microsoft.Practices.Unity;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Service;

namespace UIT.NoSQL.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private const string RavenSessionKey = "RavenMVC.Session";
        private static string databaseName;
        private static IDocumentStore documentStore;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        public MvcApplication()
        {
            //Create a DocumentSession on BeginRequest   
            //create a document session for every unit of work
            BeginRequest += (sender, args) => HttpContext.Current.Items[RavenSessionKey] = documentStore.OpenSession(databaseName);
            //Destroy the DocumentSession on EndRequest
            EndRequest += (o, eventArgs) =>
            {
                var disposable = HttpContext.Current.Items[RavenSessionKey] as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            };
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.RegisterTemplateBundles();

            //Create a DocumentStore in Application_Start
            //DocumentStore should be created once per application and stored as a singleton.
            databaseName = System.Configuration.ConfigurationManager.AppSettings["databaseName"];
            string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectRavenDB"].ConnectionString;
            //connection += "/databases/" + databaseName;
            //Raven.Client.Extensions.MultiDatabase.CreateDatabaseDocument(databaseName);
            
            
            documentStore = new DocumentStore { Url = connection };
            documentStore.Initialize();

            ConfigureUnity();

            new Utility(documentStore.OpenSession(databaseName)).Initialized();
        }

        //Getting the current DocumentSession
        public static IDocumentSession CurrentSession
        {
            get { return (IDocumentSession)HttpContext.Current.Items[RavenSessionKey]; }
        }

        private void ConfigureUnity()
        {
            //Create UnityContainer          
            IUnityContainer container = new UnityContainer()
            .RegisterType<IDocumentSession>(new InjectionFactory(c => MvcApplication.CurrentSession))
            .RegisterType<IUserService, UserService>()
            .RegisterType<IGroupService, GroupService>()
            .RegisterType<IUserGroupService, UserGroupService>()
            .RegisterType<ITopicService, TopicService>()
            .RegisterType<IGroupRoleService, GroupRoleService>();

            //Set container for Controller Factory
            Factory.MvcUnityContainer.Container = container;
            //Set Controller Factory as UnityControllerFactory
            ControllerBuilder.Current.SetControllerFactory(
                                typeof(Factory.UnityControllerFactory));
        }

    }
}