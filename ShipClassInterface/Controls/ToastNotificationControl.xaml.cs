using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ShipClassInterface.Services;

namespace ShipClassInterface.Controls
{
    public partial class ToastNotificationControl : UserControl
    {
        public ToastNotificationControl()
        {
            InitializeComponent();
            DataContext = new ToastNotificationControlViewModel();
        }
    }

    public class ToastNotificationControlViewModel
    {
        public ToastNotificationService NotificationService => ToastNotificationService.Instance;
        
        public ICommand RemoveNotificationCommand { get; }

        public ToastNotificationControlViewModel()
        {
            RemoveNotificationCommand = new RelayCommand<ToastNotification>(notification =>
            {
                if (notification != null)
                {
                    ToastNotificationService.Instance.RemoveNotification(notification);
                }
            });
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke((T?)parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute((T?)parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}