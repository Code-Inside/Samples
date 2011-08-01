using System;

namespace CastleWindsorFindAllImplementations.Commands
{
    public class FooCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("FooCommand");
        }
    }
}