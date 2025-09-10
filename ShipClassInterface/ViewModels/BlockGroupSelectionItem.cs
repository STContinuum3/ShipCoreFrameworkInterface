using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace ShipClassInterface.ViewModels
{
    public class BlockGroupSelectionItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        private readonly ObservableCollection<string> _parentCollection;

        public string Name { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                    UpdateParentCollection();
                }
            }
        }

        public BlockGroupSelectionItem(string name, ObservableCollection<string> parentCollection, bool isSelected = false)
        {
            Name = name;
            _parentCollection = parentCollection;
            _isSelected = isSelected;
        }

        private void UpdateParentCollection()
        {
            if (_isSelected && !_parentCollection.Contains(Name))
            {
                _parentCollection.Add(Name);
            }
            else if (!_isSelected && _parentCollection.Contains(Name))
            {
                _parentCollection.Remove(Name);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}