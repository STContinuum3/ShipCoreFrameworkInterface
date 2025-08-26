using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ShipClassInterface.Dialogs;
using ShipClassInterface.Models;
using ShipClassInterface.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace ShipClassInterface.ViewModels
{
    public partial class WorldConfigViewModel : ObservableObject
    {
        private readonly XmlService xmlService = new();
        private readonly ToastNotificationService toastService = ToastNotificationService.Instance;
        private readonly ShipCoreRepositoryService shipCoreRepository = ShipCoreRepositoryService.Instance;

        public WorldConfigViewModel()
        {
            // Subscribe to changes in the ship core repository
            shipCoreRepository.PropertyChanged += OnShipCoreRepositoryChanged;
        }

        private void OnShipCoreRepositoryChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ShipCoreRepositoryService.AvailableShipCores))
            {
                OnPropertyChanged(nameof(AvailableShipCoreSubtypes));
            }
        }

        [ObservableProperty]
        private WorldConfig worldConfig = new();


        [ObservableProperty]
        private NoFlyZone? selectedNoFlyZone;


        [ObservableProperty]
        private string currentFilePath = string.Empty;

        public ObservableCollection<string> AvailableShipCoreSubtypes
        {
            get
            {
                var subtypes = new ObservableCollection<string>();
                foreach (var core in shipCoreRepository.GetAllShipCores())
                {
                    subtypes.Add(core.SubtypeId);
                }
                return subtypes;
            }
        }

        [RelayCommand]
        private void NewConfiguration()
        {
            // Create a new blank world configuration
            var newWorldConfig = new WorldConfig
            {
                DebugMode = false,
                CombatLogging = false,
                LOG_LEVEL = 3,
                CLIENT_OUTPUT_LOG_LEVEL = 2,
                MaxPossibleSpeedMetersPerSecond = 300,
                IncludeAiFactions = false,
                IgnoreFactionTags = new System.Collections.ObjectModel.ObservableCollection<string>(),
                NoFlyZones = new System.Collections.ObjectModel.ObservableCollection<NoFlyZone>()
            };

            WorldConfig = newWorldConfig;
            SelectedNoFlyZone = null;
            CurrentFilePath = string.Empty;
            
            toastService.ShowSuccess("New Configuration", 
                "Created new world configuration. Configure settings and save when ready!");
        }

        [RelayCommand]
        private void LoadConfiguration()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Load World Configuration"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var config = xmlService.LoadWorldConfig(dialog.FileName);
                    if (config != null)
                    {
                        WorldConfig = config;
                        SelectedNoFlyZone = WorldConfig.NoFlyZones.FirstOrDefault();
                        CurrentFilePath = dialog.FileName;
                    }
                }
                catch (Exception ex)
                {
                    toastService.ShowError("Load Failed", 
                        $"Error loading configuration: {ex.Message}");
                }
            }
        }

        [RelayCommand]
        private void SaveConfiguration()
        {
            if (string.IsNullOrEmpty(CurrentFilePath))
            {
                SaveAsConfiguration();
                return;
            }

            try
            {
                xmlService.SaveWorldConfig(WorldConfig, CurrentFilePath);
                toastService.ShowSuccess("Saved", 
                    "World configuration saved successfully!");
            }
            catch (Exception ex)
            {
                toastService.ShowError("Save Failed", 
                    $"Error saving configuration: {ex.Message}");
            }
        }

        [RelayCommand]
        private void SaveAsConfiguration()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Save World Configuration",
                FileName = "ShipCoreConfig_World.xml"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    xmlService.SaveWorldConfig(WorldConfig, dialog.FileName);
                    CurrentFilePath = dialog.FileName;
                    toastService.ShowSuccess("Saved", 
                        "World configuration saved successfully!");
                }
                catch (Exception ex)
                {
                    toastService.ShowError("Save Failed", 
                        $"Error saving configuration: {ex.Message}");
                }
            }
        }

        [RelayCommand]
        private void AddNoFlyZone()
        {
            var newZone = new NoFlyZone
            {
                ID = WorldConfig.NoFlyZones.Count + 1,
                Radius = 1000
            };
            WorldConfig.NoFlyZones.Add(newZone);
            SelectedNoFlyZone = newZone;
        }

        [RelayCommand]
        private void RemoveNoFlyZone()
        {
            if (SelectedNoFlyZone != null)
            {
                WorldConfig.NoFlyZones.Remove(SelectedNoFlyZone);
                SelectedNoFlyZone = WorldConfig.NoFlyZones.FirstOrDefault();
            }
        }

        [RelayCommand]
        private void AddIgnoreFactionTag()
        {
            WorldConfig.IgnoreFactionTags.Add("NEW_TAG");
        }

        [RelayCommand]
        private void RemoveIgnoreFactionTag(string? tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                WorldConfig.IgnoreFactionTags.Remove(tag);
            }
        }

        [RelayCommand]
        private void AddAllowedCore()
        {
            if (SelectedNoFlyZone != null)
            {
                SelectedNoFlyZone.AllowedCoresSubtype.Add("CoreSubtype");
            }
        }

        [RelayCommand]
        private void RemoveAllowedCore(string? core)
        {
            if (SelectedNoFlyZone != null && !string.IsNullOrEmpty(core))
            {
                SelectedNoFlyZone.AllowedCoresSubtype.Remove(core);
            }
        }

    }
}