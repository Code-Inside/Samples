using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BlogPosts.Buzzwords.BusinessEntities;

namespace BlogPosts.Buzzwords.DataAccess
{
    /// <summary>
    /// Construct a Buzzword object from a datareader.
    /// </summary>
    internal class GetbuzzwordsByidFactory : IDomainObjectFactory<Buzzword>
    {
        public Buzzword Construct(IDataReader reader)
        {
            Buzzword buzzword = new Buzzword();

            int idIndex = reader.GetOrdinal("id");
            if (!reader.IsDBNull(idIndex))
            {
                buzzword.id = reader.GetInt32(idIndex);

            }

            int nameIndex = reader.GetOrdinal("name");
            if (!reader.IsDBNull(nameIndex))
            {
                buzzword.name = reader.GetString(nameIndex);

            }


            return buzzword;
        }
    }
}

