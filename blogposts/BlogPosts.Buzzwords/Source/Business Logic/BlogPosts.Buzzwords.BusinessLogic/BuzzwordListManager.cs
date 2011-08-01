using System;
using System.Collections.Generic;
using System.Text;
using BlogPosts.Buzzwords.BusinessEntities;
using BlogPosts.Buzzwords.DataAccess;

namespace BlogPosts.Buzzwords.BusinessLogic
{
    public class BuzzwordListManager
    {
        public List<Buzzword> Load()
        {
            BuzzwordRepository rep = new BuzzwordRepository("Buzzword");
            List<Buzzword> result = rep.GetAllFrombuzzwords();
            if (result == null)
            {
                // Not found
                throw new NotImplementedException();
            }
            return result;
        }
    }
}
