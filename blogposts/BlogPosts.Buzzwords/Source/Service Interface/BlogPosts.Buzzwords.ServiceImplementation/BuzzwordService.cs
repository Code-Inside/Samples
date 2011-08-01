using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using BlogPosts.Buzzwords.ServiceContracts;
using BlogPosts.Buzzwords.BusinessLogic;
using BlogPosts.Buzzwords.BusinessEntities;

namespace BlogPosts.Buzzwords.ServiceImplementation
{
    [ServiceBehavior(Name = "BuzzwordService", Namespace = "http://BlogPosts.Buzzwords.ServiceContracts/2007/10")]
    public class BuzzwordService : BlogPosts.Buzzwords.ServiceContracts.IBuzzwordService
    {
        #region IBuzzwordService Member

        public void Insert(BlogPosts.Buzzwords.DataContracts.Buzzword request)
        {
            Buzzword BuzzwordEntity = TranslateBetweenBuzzwordAndBuzzword.TranslateBuzzwordToBuzzword(request);
            BuzzwordManager Manager = new BuzzwordManager();
            Manager.Insert(BuzzwordEntity);
        }

        public BlogPosts.Buzzwords.DataContracts.Buzzword Load(int request)
        {
            BuzzwordManager Manager = new BuzzwordManager();
            Buzzword BuzzwordEntity = Manager.Load(request);
            return TranslateBetweenBuzzwordAndBuzzword.TranslateBuzzwordToBuzzword(BuzzwordEntity);
        }

        #endregion
    }
}
