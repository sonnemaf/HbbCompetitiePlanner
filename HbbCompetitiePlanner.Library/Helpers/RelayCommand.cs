using System;
using System.ComponentModel;
using System.Windows.Input;

#nullable disable

namespace ReflectionIT.Universal.Helpers {

    public class RelayCommand : RelayCommand<object>, INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public RelayCommand(Action executeAction)
            : base((o) => executeAction()) {
        }

        public RelayCommand(Action executeAction, Func<bool> canExecute)
            : base((o) => executeAction?.Invoke(), (o) => canExecute?.Invoke() ?? true) {
        }

        protected override void OnCanExecuteChanged() {
            base.OnCanExecuteChanged();
            OnPropertyChanged(nameof(IsExecutable));
        }

        public bool IsExecutable => this.CanExecute(null);
    }

    public class RelayCommand<T> : IRelayCommand {

        public event EventHandler CanExecuteChanged;

        private readonly Func<T, bool> _canExecute = null;
        private readonly Action<T> _executeAction = null;

        public RelayCommand(Action<T> executeAction) {
            _executeAction = executeAction;
        }

        public RelayCommand(Action<T> executeAction, Func<T, bool> canExecute)
            : this(executeAction) {
            _canExecute = canExecute;
        }

        public bool CanExecute(T parameter) {
            try {
                return _canExecute?.Invoke(parameter) ?? true;
            } catch { }

            return true;
        }

        public void Execute(T parameter) {
            _executeAction?.Invoke(parameter);

            OnCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged() {
            OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged() {
            try {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            } catch { }
        }

        #region ICommand Members

        bool ICommand.CanExecute(object parameter) {
            return parameter is T ? this.CanExecute((T)(parameter ?? default(T))) : this.CanExecute(default);
        }

        void ICommand.Execute(object parameter) {
            if (parameter is T) {
                this.Execute((T)parameter);
            } else {
                this.Execute(default);
            }
        }

        #endregion
    }
}
