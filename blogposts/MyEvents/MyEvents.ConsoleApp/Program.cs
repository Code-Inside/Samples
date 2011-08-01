using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyEvents.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionManager man = new ConnectionManager();
            Console.WriteLine("Start state: " + man.State.ToString());
            man.StateChanged += new ConnectionManager.StateChangedEventHandler(man_OnStateChanged);
            man.State = ConnectionStates.Connecting;
            man.State = ConnectionStates.Connected;
            man.State = ConnectionStates.Disconnected;
            Console.ReadLine();
        }

        static void man_OnStateChanged(object sender, StateChangedEventArgs e)
        {
            Console.WriteLine("State changed...");
            Console.WriteLine("New state is: " + e.NewConnectionStates.ToString());
        }
    }
}
