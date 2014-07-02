using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Jobs;

namespace WebJobPlayGround
{
    class Program
    {
        static void Main(string[] args)
        {
            JobHost host = new JobHost();
            host.Call(typeof(Program).GetMethod("WriteFile"));
            //host.RunAndBlock();
        }

        [NoAutomaticTrigger]
        public static void WriteFile([Blob("container/foobar.txt")]TextWriter writer)
        {
            writer.WriteLine("Hello World..." + DateTime.UtcNow.ToShortDateString() + " - " + DateTime.UtcNow.ToShortTimeString());
        }
    }
}
