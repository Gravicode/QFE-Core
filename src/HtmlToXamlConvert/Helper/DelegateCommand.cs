using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace HtmlToXamlConvert.Helper
{
    /// <summary>
    /// The concept for this class comes from PRISM (Composite Application Guidance)
    /// </summary>
    /// <typeparam name="T">The type of the Parameter to use for the command</typeparam>
    public class DelegateCommand<T> : ICommand
    {

        private Action<T> _command;
        private Func<T, bool> _canExecute;

        public DelegateCommand(Action<T> command)
            : this(command, (x) => true)
        {

        }

        public DelegateCommand(Action<T> command, Func<T, bool> canExecute)
        {
            _command = command;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke((T)parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
                _command.Invoke((T)parameter);
        }

        #endregion
    }
}
