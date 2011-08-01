using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace BlogPosts.Buzzwords.DataAccess
{
    /// <summary>
    /// A generic repository base class that uses various
    /// factory classes to get and retrieve specific domain
    /// objects.
    /// </summary>
    public class Repository<TDomainObject>
    {
        private string databaseName;
        private Database db;

        public Repository(string databaseName)
        {
            this.databaseName = databaseName;
            this.db = DatabaseFactory.CreateDatabase(databaseName);
        }

        /// <summary>
        /// Find all objects that match the given criteria.
        /// </summary>
        /// <typeparam name="TIdentity">Type of object used to identify
        /// the objects to find.</typeparam>
        /// <param name="selectionFactory">Factory object that can turn the
        /// identity object into the appropriate DbCommand.</param>
        /// <param name="domainObjectFactory">Factory object that can turn the
        /// returned result set into a domain object.</param>
        /// <param name="identity">Object that identifies which items to get.</param>
        /// <returns></returns>
        public List<TDomainObject> Find<TIdentity>(
            ISelectionFactory<TIdentity> selectionFactory,
            IDomainObjectFactory<TDomainObject> domainObjectFactory,
            TIdentity identity)
        {
            List<TDomainObject> results = new List<TDomainObject>();

            using (DbCommand command = selectionFactory.ConstructSelectCommand(db, identity))
            {
                using (IDataReader rdr = db.ExecuteReader(command))
                {
                    while (rdr.Read())
                    {
                        results.Add(domainObjectFactory.Construct(rdr));
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Find the first / only object that matches the given criteria.
        /// </summary>
        /// <typeparam name="TIdentity">Type of object used to identity the domain object desired.</typeparam>
        /// <param name="selectionFactory">Factory object that can turn
        /// the identity object into the appropriate DbCommand.</param>
        /// <param name="domainObjectFactory">Factory object that can turn the
        /// returned result set into a domain object.</param>
        /// <param name="identity">Object that identifies which item to get.</param>
        /// <returns>The domain object requested, or null if not found.</returns>
        public TDomainObject FindOne<TIdentity>(
            ISelectionFactory<TIdentity> selectionFactory,
            IDomainObjectFactory<TDomainObject> domainObjectFactory,
            TIdentity identity)
        {
            TDomainObject result = default(TDomainObject);
            using (DbCommand command = selectionFactory.ConstructSelectCommand(db, identity))
            {
                using (IDataReader rdr = db.ExecuteReader(command))
                {
                    if (rdr.Read())
                    {
                        result = domainObjectFactory.Construct(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Insert the given object into the database.
        /// </summary>
        /// <param name="insertFactory">Factory used to create the command.</param>
        /// <param name="domainObj">Domain object to insert</param>
        public void Add(IInsertFactory<TDomainObject> insertFactory,
            TDomainObject domainObj)
        {
            using (DbCommand command = insertFactory.ConstructInsertCommand(db, domainObj))
            {
                db.ExecuteNonQuery(command);
                insertFactory.SetNewID(db, command, domainObj);
            }
        }

        public void Remove<TIdentityObject>(IDeleteFactory<TIdentityObject> deleteFactory,
            TIdentityObject identityObj)
        {
            using (DbCommand command = deleteFactory.ConstructDeleteCommand(db, identityObj))
            {
                db.ExecuteNonQuery(command);
            }
        }

        public void Save(IUpdateFactory<TDomainObject> updateFactory, TDomainObject domainObj)
        {
            using (DbCommand command = updateFactory.ConstructUpdateCommand(db, domainObj))
            {
                db.ExecuteNonQuery(command);
            }
        }
    }
}
