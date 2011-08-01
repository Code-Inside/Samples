using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeTier.Data.DataAccess.DemoRepository
{
    public class DemoUserRepository : IUserRepository
    {
        public User GetUser(string name)
        {
            List<User> allUsers = this.GetUsers().ToList();
            return allUsers.SingleOrDefault(x => x.Login == name);
        }

        public IList<User> GetUsers()
        {
            List<User> returnList = new List<User>();
            returnList.Add(new User { Id = 1, Login = "Hans", Password = "Hans" });
            returnList.Add(new User { Id = 2, Login = "Peter", Password = "Peter" });
            returnList.Add(new User { Id = 3, Login = "Michael", Password = "Michael" });
            returnList.Add(new User { Id = 4, Login = "Hannes", Password = "Hannes" });
            returnList.Add(new User { Id = 5, Login = "Robert", Password = "Robert" });
            returnList.Add(new User { Id = 6, Login = "Oli", Password = "Oli" });
            return returnList;
        }

    }
}
