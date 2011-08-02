using System;

namespace CastleWindsorFindAllImplementations.Commands
{
    public class BarCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("BarCommand");
        }
    }
}