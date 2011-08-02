using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Repository;

namespace Service
{
    public class UserService
    {
        public ServiceResponse<User> GetUser(ServiceRequest<string> userName)
        {
            // simulate Usererror 
            if (String.IsNullOrEmpty(userName.Value))
            {
                return new ServiceResponse<User>()
                {
                    Result = ServiceResult.Failed,
                    ErrorCodes = ErrorCodes.InvalidName
                };
            }

            // simulate Systemerror
            if (userName.Value == "Tester0")
            {
                return new ServiceResponse<User>()
                {
                    Result = ServiceResult.Fatal,
                    Exception = new ArgumentException()
                };
                // or should I throw it???
            }

            // everything works
            UserRepository rep = new UserRepository();
            User result = rep.GetUsers().ToList().Where(x => x.Name == userName.Value).SingleOrDefault();
            // nothing found
            if (result == null)
            {
                return new ServiceResponse<User>() { Result = ServiceResult.Failed, ErrorCodes = ErrorCodes.NothingFound };
            }
            else
            {
                return new ServiceResponse<User>() { Value = result, Result = ServiceResult.Succeeded };
            }
        }
    }

}
