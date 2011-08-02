using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeTier.Data;

namespace ThreeTier.Service
{
    public interface IUserService
    {
        bool Login(string login, string password);
        IList<User> GetFriendsFromUser(string user);
    }
}
