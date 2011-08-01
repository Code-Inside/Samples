using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyEvents.ConsoleApp
{
    public class ConnectionManager
    {
        private ConnectionStates _state;

        public ConnectionStates State
        {
            get
            {
                return _state;
                
            }
            set
            {
                _state = value;
                OnStateChanged();
            }
        }

        public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);

        public event StateChangedEventHandler StateChanged;

        protected void OnStateChanged()
        {
            if (StateChanged != null)
            {
                StateChangedEventArgs args = new StateChangedEventArgs();
                args.NewConnectionStates = this.State;
                StateChanged(this, args);
            }
        }
        public ConnectionManager()
        {
            this.State = ConnectionStates.Disconnected;
        }
    }
}
