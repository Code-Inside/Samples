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
    /// This class is responsible for turning a selection criteria
    /// into a select statement and database query parameters.
    /// </summary>

    internal class GetAllFrombuzzwordsSelectionFactory : ISelectionFactory<NullableIdentity>
    {
        /// <summary>
        /// Creates the GetAllFrombuzzwordsSelectionFactory to build a select statement for
        /// the given .
        /// </summary>
        /// <param name=""> to select from the database.</param>      
        public GetAllFrombuzzwordsSelectionFactory()
        {
        }

        #region ISelectionFactory<NullableIdentity> Members
        public DbCommand ConstructSelectCommand(Database db, NullableIdentity nullableIdentity)
        {

            DbCommand command = db.GetStoredProcCommand("dbo.GetAllFrombuzzwords");
            return command;
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
                default:
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, GenericResources.InvalidParameterName), dbParameter);
            }
        }
        #endregion
    }
}

