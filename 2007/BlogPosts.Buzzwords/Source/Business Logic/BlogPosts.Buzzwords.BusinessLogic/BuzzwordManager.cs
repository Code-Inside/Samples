using System;
using System.Collections.Generic;
using System.Text;
using BlogPosts.Buzzwords.BusinessEntities;
using BlogPosts.Buzzwords.DataAccess;

namespace BlogPosts.Buzzwords.BusinessLogic
{
    public class BuzzwordManager
    {
        public BuzzwordManager()
        {

        }

        public Buzzword Load(int id)
        {
            BuzzwordRepository rep = new BuzzwordRepository("Buzzword");
            Buzzword result = rep.GetbuzzwordsByid(id);
            if (result == null)
            {
                // Not found
                throw new NotImplementedException();
            }
            return result;
        }

        public void Insert(Buzzword word)
        {
            BuzzwordRepository rep = new BuzzwordRepository("Buzzword");
            rep.Add(word);
        }

    }
}
