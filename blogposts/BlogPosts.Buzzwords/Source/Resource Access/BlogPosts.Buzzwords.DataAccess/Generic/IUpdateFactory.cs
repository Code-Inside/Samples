using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace BlogPosts.Buzzwords.DataAccess
{
    public interface IUpdateFactory<TDomainObject> : IDbToBusinessEntityNameMapper
    {
        DbCommand ConstructUpdateCommand(Database db, TDomainObject domainObject);
    }
}
