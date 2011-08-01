using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace MSMQ
{
    public class User
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Simple Text");
            Console.WriteLine("Input for MSMQ Message:");
            string input = Console.ReadLine();
            
            MessageQueue queue = new MessageQueue(@".\private$\test");

            queue.Send(input);

            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();

            Console.WriteLine("Output for MSMQ Queue");
            Console.WriteLine(queue.Receive().Body.ToString());
            Console.ReadLine();

            Console.WriteLine("Complex Text");

            User tester = new User();
            tester.Birthday = DateTime.Now;
            tester.Name = "Test Name";
            queue.Send(tester);

            Console.WriteLine("Output for MSMQ Queue");
            User output = (User)queue.Receive().Body;
            Console.WriteLine(output.Birthday.ToShortDateString());
            Console.ReadLine();


        }
    }
}
