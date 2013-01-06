using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceAndDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Foobar();

            Trace.WriteLine("Hello Tracing-World!");

            Debug.WriteLine("Hello Debug-World!");
        }

        [Conditional("DEBUG")]
        public static void Foobar()
        {
            Console.WriteLine("Foobar with DEBUG");
        }

    }
}
