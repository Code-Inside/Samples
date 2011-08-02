using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;

namespace HelloMEF.App
{
    class Program
    {
        static void Main(string[] args)
        {
            HelloProgram program = new HelloProgram();
            program.WriteHelloGreetings();

            Console.ReadLine();
        }
    }
}
