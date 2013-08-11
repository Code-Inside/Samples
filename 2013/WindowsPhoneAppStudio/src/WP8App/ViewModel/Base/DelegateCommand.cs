namespace WPAppStudio.ViewModel.Base
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// ICommand implementation to reuse among all our ViewModels.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Constructor without can execute.
        /// </summary>
        /// <param name="execute">Action to be launched when the command is executed.</param>
        public DelegateCommand(Action execute)
            : this(execute, null) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="execute">Action to be launched when the command is executed.</param>
        /// <param name="canExecute">Func to be executed to evaluate if a command can or can´t be executed.</param>
        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// ICommand call this method to evaluate if the command can be executed.
        /// When called, invoke the Func we have stored in canExecute if it is null return always true.
        /// </summary>
        /// <param name="parameter">Command parameter, ignored in this implementation.</param>
        /// <returns>True if the command can be execute, otherwise false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// ICommand call this method to execute the command action.
        /// When called, invoke the Action we have stored in execute if it isn´t null.
        /// </summary>
        /// <param name="parameter">Command parameter, ignored in this implementation.</param>
        public void Execute(object parameter)
        {
            if (_execute != null)
                _execute();
        }

        /// <summary>
        /// This method can be used to manually launch the command CanExecute evaluation.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
                handler(this, new EventArgs());
        }
    }

    /// <summary>
    /// ICommand generic implementation to reuse among all our ViewModels.
    /// </summary>
    /// <typeparam name="T">Type to use in the Execute and CanExecute parameters.</typeparam>
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        /// <summary>
        /// Constructor without can execute.
        /// </summary>
        /// <param name="execute">Action to be launched when the command is executed.</param>
        public DelegateCommand(Action<T> execute)
            : this(execute, null) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="execute">Action to be launched when the command is executed.</param>
        /// <param name="canExecute">Func to be executed to evaluate if a command can or can´t be executed.</param>
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// ICommand call this method to evaluate if the command can be executed.
        /// When called, invoke the Func we have stored in canExecute if it is null return always true.
        /// </summary>
        /// <param name="parameter">Command parameter, we try to cast it to T.</param>
        /// <returns>True if the command can be execute, otherwise false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// ICommand call this method to execute the command action.
        /// When called, invoke the Action we have stored in execute if it isn´t null.
        /// </summary>
        /// <param name="parameter">Command parameter, we try to cast it to T.</param>
        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                if (parameter == null)
                    _execute(default(T));
                else
                    _execute((T)parameter);
            }
        }

        /// <summary>
        /// This method can be used to manually launch the command CanExecute evaluation.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
                handler(this, new EventArgs());
        }
    }
}
