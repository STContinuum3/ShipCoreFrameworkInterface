using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ShipClassInterface.Services
{
    public enum ToastType
    {
        Success,
        Info,
        Warning,
        Error
    }

    public class ToastNotification : INotifyPropertyChanged
    {
        private bool _isVisible = true;

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ToastType Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public string TypeIcon => Type switch
        {
            ToastType.Success => "CheckCircle",
            ToastType.Info => "Information",
            ToastType.Warning => "Alert",
            ToastType.Error => "AlertCircle",
            _ => "Information"
        };

        public string TypeColor => Type switch
        {
            ToastType.Success => "#4CAF50",
            ToastType.Info => "#2196F3",
            ToastType.Warning => "#FF9800",
            ToastType.Error => "#F44336",
            _ => "#2196F3"
        };

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ToastNotificationService : INotifyPropertyChanged
    {
        private static ToastNotificationService? _instance;
        private readonly Dispatcher _dispatcher;

        public static ToastNotificationService Instance => _instance ??= new ToastNotificationService();

        public ObservableCollection<ToastNotification> Notifications { get; } = new();

        private ToastNotificationService()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void ShowSuccess(string title, string message)
        {
            ShowNotification(title, message, ToastType.Success);
        }

        public void ShowInfo(string title, string message)
        {
            ShowNotification(title, message, ToastType.Info);
        }

        public void ShowWarning(string title, string message)
        {
            ShowNotification(title, message, ToastType.Warning);
        }

        public void ShowError(string title, string message)
        {
            ShowNotification(title, message, ToastType.Error);
        }

        private void ShowNotification(string title, string message, ToastType type)
        {
            var notification = new ToastNotification
            {
                Title = title,
                Message = message,
                Type = type
            };

            if (_dispatcher.CheckAccess())
            {
                AddNotification(notification);
            }
            else
            {
                _dispatcher.Invoke(() => AddNotification(notification));
            }
        }

        private void AddNotification(ToastNotification notification)
        {
            Notifications.Insert(0, notification);

            // Auto-remove after 5 seconds
            Task.Delay(5000).ContinueWith(_ =>
            {
                _dispatcher.Invoke(() =>
                {
                    notification.IsVisible = false;
                    // Remove after fade out animation
                    Task.Delay(300).ContinueWith(__ =>
                    {
                        _dispatcher.Invoke(() => Notifications.Remove(notification));
                    });
                });
            });

            // Keep only the latest 5 notifications
            while (Notifications.Count > 5)
            {
                Notifications.RemoveAt(Notifications.Count - 1);
            }
        }

        public void RemoveNotification(ToastNotification notification)
        {
            if (_dispatcher.CheckAccess())
            {
                notification.IsVisible = false;
                Task.Delay(300).ContinueWith(_ =>
                {
                    _dispatcher.Invoke(() => Notifications.Remove(notification));
                });
            }
            else
            {
                _dispatcher.Invoke(() => RemoveNotification(notification));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}