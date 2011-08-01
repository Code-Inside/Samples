using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Timers.ConsoleApp
{


    class Program
    {


        static void Main(string[] args)
        {
        

            // Dirty:
            //while (true)
            //{
            //    Thread.Sleep(1000);
            //    Console.WriteLine("Bla!");
            //}

            // Variante "Seltsam"
            //AutoResetEvent _isStopping = new AutoResetEvent(false);
            //TimeSpan waitInterval = TimeSpan.FromMilliseconds(1000);
            //for (; !_isStopping.WaitOne(waitInterval); )
            //{
            //    Console.WriteLine("Bla!");
            //}

            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;

            Console.ReadLine();
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Elapsed!");
        }
    }
}
