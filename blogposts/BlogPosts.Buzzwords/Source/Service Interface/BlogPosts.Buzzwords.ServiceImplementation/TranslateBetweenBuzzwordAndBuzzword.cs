using System;
using BlogPosts.Buzzwords.BusinessEntities;
using BlogPosts.Buzzwords.DataContracts;

namespace BlogPosts.Buzzwords.ServiceImplementation
{
    public static class TranslateBetweenBuzzwordAndBuzzword
    {
        public static BlogPosts.Buzzwords.BusinessEntities.Buzzword TranslateBuzzwordToBuzzword(BlogPosts.Buzzwords.DataContracts.Buzzword from)
        {
            BlogPosts.Buzzwords.BusinessEntities.Buzzword to = new BlogPosts.Buzzwords.BusinessEntities.Buzzword();
            to.id = from.Id;
            to.name = from.Name;
            return to;
        }

        public static BlogPosts.Buzzwords.DataContracts.Buzzword TranslateBuzzwordToBuzzword(BlogPosts.Buzzwords.BusinessEntities.Buzzword from)
        {
            BlogPosts.Buzzwords.DataContracts.Buzzword to = new BlogPosts.Buzzwords.DataContracts.Buzzword();
            to.Id = from.id;
            to.Name = from.name;
            return to;
        }
    }
}

