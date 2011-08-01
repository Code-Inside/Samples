using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace DllMembership.Lib
{
    public class MembershipService
    {
        public IList<User> GetUsers()
        {  
            MembershipUserCollection col = Membership.GetAllUsers();
            List<User> returnList = new List<User>();
            foreach (MembershipUser user in col)
            {
                User u = new User();
                u.Name = user.UserName;
                u.Email = user.Email;
                returnList.Add(u);
            }
            return returnList;
        }

        public User Login(string username, string password)
        {
            User returnUser = new User();

            if (Membership.ValidateUser(username, password))
            {
                returnUser.IsLoggedIn = true;
                returnUser.Name = username;
                returnUser.Password = password;
                return returnUser;
            }
            else
            {
                returnUser.IsLoggedIn = false;
                return returnUser;
            }
        }

        public User GetUser(string username)
        {
            MembershipUser user = Membership.GetUser(username);
            User returnUser = new User();
            returnUser.Name = user.UserName;
            returnUser.Email = user.Email;
            return returnUser;
        }
    }
}
