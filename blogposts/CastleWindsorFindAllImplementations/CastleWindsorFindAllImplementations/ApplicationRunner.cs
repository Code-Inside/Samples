using CastleWindsorFindAllImplementations.Commands;

namespace CastleWindsorFindAllImplementations
{
    public class ApplicationRunner : IApplicationRunner
    {
        private ICommand[] _commands;

        public ApplicationRunner(ICommand[] commands)
        {
            this._commands = commands;
        }

        public void Run()
        {
            foreach (ICommand command in _commands)
            {
                command.Execute();
            }
        }
    }
}