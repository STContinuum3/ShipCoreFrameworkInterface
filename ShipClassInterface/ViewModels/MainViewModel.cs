using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShipClassInterface.Models;
using ShipClassInterface.Services;
using System.Collections.ObjectModel;
using System.IO;

namespace ShipClassInterface.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly UserSettingsService settingsService = UserSettingsService.Instance;

        [ObservableProperty]
        private ObservableObject? currentViewModel;

        [ObservableProperty]
        private int selectedIndex;

        public bool IsPinned
        {
            get => settingsService.Settings.IsNavigationPinned;
            set 
            { 
                settingsService.UpdateSetting(s => s.IsNavigationPinned = value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsDrawerOpen));
            }
        }

        public bool IsDrawerOpen => IsPinned || _isTemporarilyOpen;
        
        private bool _isTemporarilyOpen;
        public bool IsTemporarilyOpen
        {
            get => _isTemporarilyOpen;
            set
            {
                _isTemporarilyOpen = value;
                OnPropertyChanged(nameof(IsDrawerOpen));
            }
        }

        public ShipCoreViewModel ShipCoreViewModel { get; }
        public BlockGroupViewModel BlockGroupViewModel { get; }
        public WorldConfigViewModel WorldConfigViewModel { get; }
        public ManifestViewModel ManifestViewModel { get; }
        public BlockCreatorViewModel BlockCreatorViewModel { get; }
        public UnifiedXmlLoader UnifiedLoader { get; }

        public MainViewModel()
        {
            ShipCoreViewModel = new ShipCoreViewModel();
            BlockGroupViewModel = new BlockGroupViewModel();
            WorldConfigViewModel = new WorldConfigViewModel();
            ManifestViewModel = new ManifestViewModel();
            BlockCreatorViewModel = new BlockCreatorViewModel();
            UnifiedLoader = new UnifiedXmlLoader(this);
            
            CurrentViewModel = ShipCoreViewModel;
            
            // Try to auto-load block groups from standard SE location
            TryAutoLoadBlockGroups();
        }

        private void TryAutoLoadBlockGroups()
        {
            try
            {
                var seDataPath = @"C:\Users\Sysop\AppData\Roaming\SpaceEngineers\Saves\76561198037642958\test warp\Storage\ship Core Framework_ShipCoreFramework";
                
                // Try to auto-load available XML files
                var filesToTry = new[]
                {
                    Path.Combine(seDataPath, "ShipCoreConfig_Groups.xml"),
                    Path.Combine(seDataPath, "ShipCoreConfig_World.xml"),
                    Path.Combine(seDataPath, "ShipCoreConfig_No_Core.xml")
                };

                foreach (var file in filesToTry)
                {
                    if (File.Exists(file))
                    {
                        var fileType = XmlFileTypeDetector.DetectFileType(file);
                        
                        // Auto-load block groups silently for better UX
                        if (fileType == XmlFileType.BlockGroups)
                        {
                            var xmlService = new XmlService();
                            var groups = xmlService.LoadBlockGroups(file);
                            if (groups != null)
                            {
                                BlockGroupService.Instance.LoadBlockGroups(groups.BlockGroups, file);
                            }
                            break; // Only load one block groups file
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors during auto-load - user can manually load if needed
            }
        }

        partial void OnSelectedIndexChanged(int value)
        {
            CurrentViewModel = value switch
            {
                0 => ShipCoreViewModel,
                1 => BlockGroupViewModel,
                2 => WorldConfigViewModel,
                3 => ManifestViewModel,
                4 => BlockCreatorViewModel,
                _ => ShipCoreViewModel
            };
        }

        [RelayCommand]
        private void NavigateToShipCore()
        {
            SelectedIndex = 0;
        }

        [RelayCommand]
        private void NavigateToBlockGroups()
        {
            SelectedIndex = 1;
        }

        [RelayCommand]
        private void NavigateToWorldConfig()
        {
            SelectedIndex = 2;
        }

        [RelayCommand]
        private void NavigateToManifest()
        {
            SelectedIndex = 3;
        }

        [RelayCommand]
        private void NavigateToBlockCreator()
        {
            SelectedIndex = 4;
        }

        [RelayCommand]
        private async Task LoadAnyXmlFile()
        {
            UnifiedLoader.LoadXmlFile();
        }

        [RelayCommand]
        private void LoadSpecificXmlFile(string filePath)
        {
            UnifiedLoader.LoadXmlFile(filePath);
        }
    }
}