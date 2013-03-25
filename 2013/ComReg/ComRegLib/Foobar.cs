using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ComRegLib
{
    [ComVisible(true)]
    [Guid("E041712F-D936-4B5B-A3F0-5DB66C4634B0"), ProgId("Foobar")]
    public class Foobar
    {
        [ComRegisterFunction]
        public static void RegisterFunction(Type type)
        {
            Console.WriteLine("Register of Foobar...");
        }

        [ComUnregisterFunction]
        public static void UnregisterFuntion(Type type)
        {
            Console.WriteLine("Unregister of Foobar...");
        }

        public string Buzz()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
