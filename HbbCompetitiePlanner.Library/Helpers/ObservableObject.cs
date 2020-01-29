using System.ComponentModel;

namespace ReflectionIT.Universal.Helpers {

    public abstract class ObservableObject : INotifyPropertyChanged {

        [System.Diagnostics.DebuggerStepThrough]
        protected ObservableObject() {
            // chaining
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
