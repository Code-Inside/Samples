using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyEvents.ConsoleApp
{
    public class StateChangedEventArgs : EventArgs
    {
        public ConnectionStates NewConnectionStates { get; set; }
    }
}
