using System;
using System.Collections.Generic;
using System.Text;

namespace UsingInterfaces.Nature
{
    public class Human : IMovable
    {
        #region IMovable Member

        public void Move()
        {
            Console.WriteLine("Der Mensch macht einen Schritt vor.");
        }

        #endregion
    }
}
