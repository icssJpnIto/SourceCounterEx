using System;
using System.Diagnostics;
using System.Windows.Input;

namespace SourceCounterEx.Command
{
    public class RelayCommand : IRelayCommand
    {
        private readonly Action action;
        private readonly Func<bool> canExecute;

        protected RelayCommand()
        {
        }

        public RelayCommand(Action execute)
        {
            this.action = execute;
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
            : this(execute)
        {
            this.canExecute = canExecute;
        }

        #region IRelayCommand Members

        [DebuggerStepThrough]
        public virtual bool CanExecute(object parameter)
        {
            return this.canExecute == null
                || this.canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public event EventHandler Executed;

        public event EventHandler Executing;

        protected void OnExecuted()
        {
            if (this.Executed != null)
            {
                this.Executed(this, null);
            }
        }

        protected void OnExecuting()
        {
            if (this.Executing != null)
            {
                this.Executing(this, null);
            }
        }

        public void Execute(object parameter)
        {
            this.OnExecuting();

            this.InvokeAction(parameter);

            this.OnExecuted();
        }

        protected virtual void InvokeAction(object parameter)
        {
            this.action();
        }

        #endregion // IRelayCommand Members
    }

    public class RelayCommand<T> : RelayCommand
    {
        private readonly Action<T> action;
        private readonly Func<T, bool> canExecute;

        protected RelayCommand()
        {
        }

        public RelayCommand(Action<T> action)
            : this(action, null)
        {
        }

        public RelayCommand(Action<T> action, Func<T, bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return this.canExecute == null
                   || this.canExecute((T)parameter);
        }

        protected override void InvokeAction(object parameter)
        {
            this.action((T)parameter);
        }
    }
}
