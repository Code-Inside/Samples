using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CallerInformationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Log("Hello World...");
            Console.ReadLine();
        }

        public static void Log(string text, [CallerMemberName] string callerMemberName = "",
                                            [CallerFilePath] string callerPath = "", 
                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            Console.WriteLine("Invoked with: " + text);
            Console.WriteLine("Caller {0} from File {1} (Ln: {2})", callerMemberName, callerPath, callerLineNumber);
        }
    }
}
