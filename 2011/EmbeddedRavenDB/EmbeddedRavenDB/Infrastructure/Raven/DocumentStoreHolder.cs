using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace EmbeddedRavenDB.Infrastructure.Raven
{
    /// <summary>
    /// This class manages the state of objects that desire a document session. We aren't relying on an IoC container here
    /// because this is the sole case where we actually need to do injection.
    /// </summary>
    public class DocumentStoreHolder
    {
        private static IDocumentStore documentStore;

        public static IDocumentStore DocumentStore
        {
            get { return (documentStore ?? (documentStore = CreateDocumentStore())); }
        }

        private static IDocumentStore CreateDocumentStore()
        {
            var documentStore = new EmbeddableDocumentStore
            {
                ConnectionStringName = "RavenDB"
            }.Initialize();

            return documentStore;
        }

        private static readonly ConcurrentDictionary<Type, Accessors> AccessorsCache = new ConcurrentDictionary<Type, Accessors>();


        private static Accessors CreateAccessorsForType(Type type)
        {
            var sessionProp =
                type.GetProperties().FirstOrDefault(
                    x => x.PropertyType == typeof(IDocumentSession) && x.CanRead && x.CanWrite);
            if (sessionProp == null)
                return null;

            return new Accessors
            {
                Set = (instance, session) => sessionProp.SetValue(instance, session, null),
                Get = instance => (IDocumentSession)sessionProp.GetValue(instance, null)
            };
        }


        public static IDocumentSession TryAddSession(object instance)
        {
            var accessors = AccessorsCache.GetOrAdd(instance.GetType(), CreateAccessorsForType);

            if (accessors == null)
                return null;

            var documentSession = DocumentStore.OpenSession();
            accessors.Set(instance, documentSession);

            return documentSession;
        }

        public static void TryComplete(object instance, bool succcessfully)
        {
            Accessors accesors;
            if (AccessorsCache.TryGetValue(instance.GetType(), out accesors) == false || accesors == null)
                return;

            using (var documentSession = accesors.Get(instance))
            {
                if (documentSession == null)
                    return;

                if (succcessfully)
                    documentSession.SaveChanges();
            }
        }

        private class Accessors
        {
            public Action<object, IDocumentSession> Set;
            public Func<object, IDocumentSession> Get;
        }

        public static void Initailize()
        {
            //RavenProfiler.InitializeFor(DocumentStore,
            //    //Fields to filter out of the output
            //    "Email", "HashedPassword", "AkismetKey", "GoogleAnalyticsKey", "ShowPostEvenIfPrivate", "PasswordSalt", "UserHostAddress");

        }

        public static void TrySetSession(object instance, IDocumentSession documentSession)
        {
            var accessors = AccessorsCache.GetOrAdd(instance.GetType(), CreateAccessorsForType);

            if (accessors == null)
                return;

            accessors.Set(instance, documentSession);
        }
    }
}