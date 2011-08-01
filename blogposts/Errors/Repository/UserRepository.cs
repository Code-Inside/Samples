using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace Repository
{
    public class UserRepository
    {
        public IList<User> GetUsers()
        {
            List<User> returnList = new List<User>();
            returnList.Add(new User() { Id = Guid.NewGuid(), Name = "Tester0" });
            returnList.Add(new User() { Id = Guid.NewGuid(), Name = "Tester1" });
            returnList.Add(new User() { Id = Guid.NewGuid(), Name = "Tester2" });
            returnList.Add(new User() { Id = Guid.NewGuid(), Name = "Tester3" });
            returnList.Add(new User() { Id = Guid.NewGuid(), Name = "Tester4" });

            return returnList;
        }
    }
}
