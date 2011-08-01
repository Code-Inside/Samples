using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;

namespace BlogPosts.Buzzwords.DataAccess
{
    public interface IDeleteFactory<TIdentityObject> : IDbToBusinessEntityNameMapper
    {
        DbCommand ConstructDeleteCommand(Database db, TIdentityObject identity);
    }
}
