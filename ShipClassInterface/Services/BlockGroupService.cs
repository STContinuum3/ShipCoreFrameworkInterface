using ShipClassInterface.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ShipClassInterface.Services
{
    public class BlockGroupService : INotifyPropertyChanged
    {
        private static BlockGroupService? _instance;
        public static BlockGroupService Instance => _instance ??= new BlockGroupService();

        private ObservableCollection<BlockGroup> _availableBlockGroups = new();
        private ObservableCollection<string> _blockGroupNames = new();
        private string _currentFilePath = string.Empty;

        public ObservableCollection<BlockGroup> AvailableBlockGroups
        {
            get => _availableBlockGroups;
            private set
            {
                _availableBlockGroups = value;
                UpdateBlockGroupNames();
                OnPropertyChanged(nameof(AvailableBlockGroups));
                OnPropertyChanged(nameof(BlockGroupNames));
            }
        }

        public ObservableCollection<string> BlockGroupNames
        {
            get => _blockGroupNames;
        }

        private void UpdateBlockGroupNames()
        {
            _blockGroupNames.Clear();
            foreach (var group in AvailableBlockGroups)
            {
                if (!string.IsNullOrEmpty(group.Name))
                {
                    _blockGroupNames.Add(group.Name);
                }
            }
        }

        public string CurrentFilePath
        {
            get => _currentFilePath;
            private set
            {
                _currentFilePath = value;
                OnPropertyChanged(nameof(CurrentFilePath));
                OnPropertyChanged(nameof(HasBlockGroups));
            }
        }

        public bool HasBlockGroups => AvailableBlockGroups.Count > 0;

        private BlockGroupService() { }

        public void LoadBlockGroups(ObservableCollection<BlockGroup> blockGroups, string filePath = "")
        {
            AvailableBlockGroups = blockGroups;
            CurrentFilePath = filePath;
        }

        public void ClearBlockGroups()
        {
            AvailableBlockGroups = new ObservableCollection<BlockGroup>();
            CurrentFilePath = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}