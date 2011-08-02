using System;
using System.Collections.Generic;
using System.Text;

namespace UsingInterfaces.Nature
{
    public class God
    {
        public static void MoveObject(IMovable item)
        {
            Console.WriteLine("Ein bewegliches Objekt wird bewegt...");
            item.Move();
        }
    }
}
