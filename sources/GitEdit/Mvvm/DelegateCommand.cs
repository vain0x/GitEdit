using System;
using System.Diagnostics;
using System.Windows.Input;

namespace GitEdit.Mvvm
{
    public sealed class DelegateCommand<TParameter>
        : ICommand
    {
        readonly Action<TParameter> _execute;
        readonly Predicate<TParameter> _canExecute;

        public DelegateCommand(Action<TParameter> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<TParameter> execute, Predicate<TParameter> canExecute)
        {
            Debug.Assert(execute != null);

            _execute = execute;
            _canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(TParameter parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        [DebuggerStepThrough]
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((TParameter)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(TParameter parameter)
        {
            _execute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            Execute((TParameter)parameter);
        }
    }
}
