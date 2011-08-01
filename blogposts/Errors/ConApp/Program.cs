using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Service;
using Model;

namespace ConApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Username");
            Console.WriteLine("(Tester0 == Systemerror)");
            Console.WriteLine("(enter nothing == Usererror)");

            string username = Console.ReadLine();

            UserService srv = new UserService();
            ServiceResponse<User> result = srv.GetUser(new ServiceRequest<string>() { Value = username });

            if (result.Result == ServiceResult.Succeeded)
            {
                Console.WriteLine("Service Call Succeeded");
                Console.WriteLine(result.Value);
            }

            if (result.Result == ServiceResult.Failed)
            {
                Console.WriteLine("Service Call Failed");
                Console.WriteLine(result.ErrorCodes.ToString());
            }

            if (result.Result == ServiceResult.Fatal)
            {
                Console.WriteLine("Service Call Fatal Error");
                Console.WriteLine(result.Exception.ToString());
            }

            Console.ReadLine();
        }
    }
}
