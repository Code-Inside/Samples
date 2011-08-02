using System;

namespace CastleWindsorFindAllImplementations.Commands
{
    public class WakeUpCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("WakeUpCommand");
        }
    }
}