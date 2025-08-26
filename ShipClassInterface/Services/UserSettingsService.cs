using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ShipClassInterface.Services
{
    public class UserSettingsService : INotifyPropertyChanged
    {
        private static UserSettingsService? _instance;
        public static UserSettingsService Instance => _instance ??= new UserSettingsService();

        private readonly string _settingsFilePath;
        private UserSettings _settings = new();

        public UserSettings Settings
        {
            get => _settings;
            private set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        private UserSettingsService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "ShipClassInterface");
            Directory.CreateDirectory(appFolder);
            _settingsFilePath = Path.Combine(appFolder, "settings.json");
            
            LoadSettings();
        }

        public void SaveSettings()
        {
            try
            {
                var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(_settingsFilePath, json);
            }
            catch (Exception ex)
            {
                ToastNotificationService.Instance.ShowError("Settings Save Failed", 
                    $"Could not save user settings: {ex.Message}");
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    var settings = JsonSerializer.Deserialize<UserSettings>(json);
                    if (settings != null)
                    {
                        Settings = settings;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ToastNotificationService.Instance.ShowWarning("Settings Load Failed", 
                    $"Could not load user settings, using defaults: {ex.Message}");
            }

            // Use default settings if loading failed
            Settings = new UserSettings();
        }

        public void UpdateSetting(Action<UserSettings> updateAction)
        {
            updateAction(Settings);
            SaveSettings();
            OnPropertyChanged(nameof(Settings));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class UserSettings
    {
        public bool IsNavigationPinned { get; set; } = false;
        public double WindowWidth { get; set; } = 1600;
        public double WindowHeight { get; set; } = 900;
        public string LastOpenedFilePath { get; set; } = string.Empty;
        public string Theme { get; set; } = "Dark";
    }
}