using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace BlogPosts.Buzzwords.DataAccess
{
    /// <summary>
    /// Interface to factory class that generates DbCommand to
    /// insert a new domain object into the database.
    /// </summary>
    /// <typeparam name="TDomainObject">Type of domain object to insert.</typeparam>
    public interface IInsertFactory<TDomainObject> : IDbToBusinessEntityNameMapper
    {
        /// <summary>
        /// Generate the insert command.
        /// </summary>
        /// <param name="db">Entlib Database object to generate command for.</param>
        /// <param name="domainObj">Domain object to insert.</param>
        /// <returns>Initialized DbCommand object.</returns>
        DbCommand ConstructInsertCommand(Database db, TDomainObject domainObj);

        /// <summary>
        /// Read the ID of the newly inserted object out of the command
        /// and set it in the domain object.
        /// </summary>
        /// <param name="db">EntLib database object that the command was generated with.</param>
        /// <param name="command">Successfully executed command that now holds the id. This should be the same command object that was returned from ConstructInsertCommand.</param>
        /// <param name="domainObj">Domain object that was passed in to ConstructInsertCommand. This will have the ID assigned by the database set.</param>
        void SetNewID(Database db, DbCommand command, TDomainObject domainObj);
    }
}
