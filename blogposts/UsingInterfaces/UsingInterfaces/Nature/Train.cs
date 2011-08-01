using System;
using System.Collections.Generic;
using System.Text;

namespace UsingInterfaces.Nature
{
    public class Train : IMovable
    {
        #region IMovable Member

        public void Move()
        {
            Console.WriteLine("Der Zug fährt bis zum nächsten Bahnhof.");
        }

        #endregion
    }
}
