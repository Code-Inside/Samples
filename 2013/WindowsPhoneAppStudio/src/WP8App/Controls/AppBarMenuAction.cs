using System;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WPAppStudio.Controls
{
    public class AppBarMenuAction
    {
        #region NotificationCommand

        private class NotificationCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Action _finish;
            private readonly Func<bool> _canExecute;

            public NotificationCommand(Action execute, Func<bool> canExecute, Action finishCommand)
            {
                _execute = execute;
                _canExecute = canExecute;
                _finish = finishCommand;
            }

            bool ICommand.CanExecute(object parameter)
            {
                return _canExecute();
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                add { }
                remove { }
            }

            void ICommand.Execute(object parameter)
            {
                _execute();
                _finish();
            }
        }

        #endregion

        public object Content { get; set; }
        internal ICommand Command { get; private set; }

        internal AppBarMenu Parent { get; set; }

        public AppBarMenuAction(object content, Action execute)
            : this(content, execute, () => true)
        {
        }

        public AppBarMenuAction(object content, Action execute, Func<bool> canExecute)
        {
            Content = content;
            Command = new NotificationCommand(execute, canExecute, () => ((Popup)Parent.Parent).IsOpen = false);
        }
    }
}
