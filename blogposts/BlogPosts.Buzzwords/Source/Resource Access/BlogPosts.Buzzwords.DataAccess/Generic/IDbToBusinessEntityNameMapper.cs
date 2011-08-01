using System;
namespace BlogPosts.Buzzwords.DataAccess
{
    public interface IDbToBusinessEntityNameMapper
    {
        string MapDbParameterToBusinessEntityProperty(string dbParameter);
    }
}
