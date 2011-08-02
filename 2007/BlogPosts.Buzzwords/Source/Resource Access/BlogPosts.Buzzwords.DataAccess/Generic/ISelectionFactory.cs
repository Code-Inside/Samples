using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace BlogPosts.Buzzwords.DataAccess
{
    /// <summary>
    /// This interface provides the signature for the
    /// class that converts a given IdentityObject into
    /// a DbCommand to do a search for the corresponding
    /// result set.
    /// </summary>
    /// <typeparam name="TIdentityObject">Type that identifies the
    /// items to search for.</typeparam>
    public interface ISelectionFactory<TIdentityObject> : IDbToBusinessEntityNameMapper
    {
        DbCommand ConstructSelectCommand(Database db, TIdentityObject idObject);
    }
}
