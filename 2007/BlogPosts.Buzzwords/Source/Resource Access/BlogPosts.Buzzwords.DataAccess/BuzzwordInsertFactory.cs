using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using BlogPosts.Buzzwords.BusinessEntities;
using BlogPosts.Buzzwords.DataAccess.Generic;
using System.Globalization;

namespace BlogPosts.Buzzwords.DataAccess
{
    /// <summary>
    /// A factory object that is responsible for taking a Buzzword
    /// and generating the corresponding SQL to insert that
    /// Buzzword into the database. It also includes a method
    /// to grab the returned ID from the call and updating the
    /// Buzzword object with it.
    /// </summary>
    internal class BuzzwordInsertFactory : IDbToBusinessEntityNameMapper, IInsertFactory<Buzzword>
    {
        /// <summary>
        /// Creates the BuzzwordInsertFactory to build an insert statement for
        /// the given Buzzword object.
        /// </summary>
        /// <param name="Buzzword">New Buzzword to insert into the database.</param>
        public BuzzwordInsertFactory()
        {
        }

        #region IInsertFactory<Buzzword> Members

        public DbCommand ConstructInsertCommand(Database db, Buzzword buzzword)
        {
            DbCommand command = db.GetStoredProcCommand("dbo.Insertbuzzwords");

            if (buzzword.name != null)
            {
                db.AddInParameter(command, "name", DbType.String, buzzword.name);
            }
            db.AddOutParameter(command, "id", DbType.Int32, 4);
            return command;
        }

        public void SetNewID(Database db, DbCommand command, Buzzword buzzword)
        {
            System.Int32 id1 = (System.Int32)(db.GetParameterValue(command, "id"));
            buzzword.id = id1;

        }

        #endregion

        #region IDbToBusinessEntityNameMapper Members
        /// <summary>
        /// Maps a field name in the database (usually a parameter name for a stored proc)
        /// to the corresponding business entity property name.
        /// </summary>
        /// <remarks>This method is intended for error message handling, not for reflection.</remarks>
        /// <param name="dbParameter">Name of field/parameter that the database knows about.</param>
        /// <returns>Corresponding business entity field name.</returns>
        public string MapDbParameterToBusinessEntityProperty(string dbParameter)
        {
            switch (dbParameter)
            {
                case "name":
                    return "name";
                default:
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, GenericResources.InvalidParameterName), dbParameter);
            }
        }
        #endregion
    }

}

