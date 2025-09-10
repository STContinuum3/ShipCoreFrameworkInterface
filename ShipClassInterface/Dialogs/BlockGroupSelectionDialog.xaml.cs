using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ShipClassInterface.Dialogs
{
    public partial class BlockGroupSelectionDialog : Window
    {
        private ObservableCollection<BlockGroupSelectionItem> _selectionItems = new();

        public ObservableCollection<string> SelectedBlockGroups { get; private set; }

        public BlockGroupSelectionDialog(ObservableCollection<string> availableGroups, ObservableCollection<string> currentSelection)
        {
            InitializeComponent();
            
            SelectedBlockGroups = new ObservableCollection<string>(currentSelection ?? new ObservableCollection<string>());
            
            // Initialize selection items
            foreach (var group in availableGroups)
            {
                var item = new BlockGroupSelectionItem
                {
                    Name = group,
                    IsSelected = SelectedBlockGroups.Contains(group)
                };
                
                // Subscribe to property changes
                item.PropertyChanged += Item_PropertyChanged;
                _selectionItems.Add(item);
            }
            
            BlockGroupsList.ItemsSource = _selectionItems;
            UpdateSelectionCount();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BlockGroupSelectionItem.IsSelected) && sender is BlockGroupSelectionItem item)
            {
                if (item.IsSelected && !SelectedBlockGroups.Contains(item.Name))
                {
                    SelectedBlockGroups.Add(item.Name);
                }
                else if (!item.IsSelected && SelectedBlockGroups.Contains(item.Name))
                {
                    SelectedBlockGroups.Remove(item.Name);
                }
                
                UpdateSelectionCount();
            }
        }

        private void UpdateSelectionCount()
        {
            var selectedCount = _selectionItems.Count(i => i.IsSelected);
            var totalCount = _selectionItems.Count;
            
            if (selectedCount == 0)
                SelectionCountText.Text = "No groups selected";
            else if (selectedCount == 1)
                SelectionCountText.Text = "1 group selected";
            else
                SelectionCountText.Text = $"{selectedCount} of {totalCount} groups selected";
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _selectionItems)
            {
                item.IsSelected = true;
            }
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _selectionItems)
            {
                item.IsSelected = false;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class BlockGroupSelectionItem : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Name { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}