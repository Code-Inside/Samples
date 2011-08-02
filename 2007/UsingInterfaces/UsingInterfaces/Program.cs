using System;
using System.Collections.Generic;
using System.Text;
using UsingInterfaces.Nature;
namespace UsingInterfaces
{
    class Program
    {
        static void Main(string[] args)
        {
            Car BMW = new Car();
            Train ICE = new Train();
            Human Robert = new Human();

            God.MoveObject(BMW);
            God.MoveObject(ICE);
            God.MoveObject(Robert);

            Console.ReadLine();
        }
    }
}
