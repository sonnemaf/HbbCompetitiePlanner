using System.Windows.Input;

namespace ReflectionIT.Universal.Helpers {
    public interface IRelayCommand : ICommand {

#pragma warning disable CA1030 // Use events where appropriate
        void RaiseCanExecuteChanged();
#pragma warning restore CA1030 // Use events where appropriate
    }
}
