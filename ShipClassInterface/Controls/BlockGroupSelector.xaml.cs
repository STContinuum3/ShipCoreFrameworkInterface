using ShipClassInterface.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ShipClassInterface.Controls
{
    public partial class BlockGroupSelector : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty SelectedBlockGroupsProperty =
            DependencyProperty.Register(nameof(SelectedBlockGroups), typeof(ObservableCollection<string>),
                typeof(BlockGroupSelector), new FrameworkPropertyMetadata(null, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedBlockGroupsChanged));

        public static readonly DependencyProperty AvailableBlockGroupsProperty =
            DependencyProperty.Register(nameof(AvailableBlockGroups), typeof(ObservableCollection<string>),
                typeof(BlockGroupSelector), new PropertyMetadata(null, OnAvailableBlockGroupsChanged));

        private ObservableCollection<BlockGroupSelectionItem> _selectionItems = new();

        public ObservableCollection<string> SelectedBlockGroups
        {
            get => (ObservableCollection<string>)GetValue(SelectedBlockGroupsProperty);
            set => SetValue(SelectedBlockGroupsProperty, value);
        }

        public ObservableCollection<string> AvailableBlockGroups
        {
            get => (ObservableCollection<string>)GetValue(AvailableBlockGroupsProperty);
            set => SetValue(AvailableBlockGroupsProperty, value);
        }

        public string SelectedText
        {
            get
            {
                if (SelectedBlockGroups == null || SelectedBlockGroups.Count == 0)
                    return "No groups selected";

                if (SelectedBlockGroups.Count == 1)
                    return SelectedBlockGroups[0];

                return $"{SelectedBlockGroups.Count} groups selected";
            }
        }

        public BlockGroupSelector()
        {
            InitializeComponent();
            BlockGroupItemsControl.ItemsSource = _selectionItems;
            UpdateSelectionCountText();
            
            // Make this control focusable to maintain DataGrid edit mode
            Focusable = true;
            
            // Subscribe to popup events for proper management
            Loaded += BlockGroupSelector_Loaded;
        }

        private static void OnSelectedBlockGroupsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockGroupSelector selector)
            {
                selector.OnSelectedBlockGroupsChanged();
            }
        }

        private static void OnAvailableBlockGroupsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockGroupSelector selector)
            {
                selector.OnAvailableBlockGroupsChanged();
            }
        }

        private void OnSelectedBlockGroupsChanged()
        {
            if (SelectedBlockGroups != null)
            {
                SelectedBlockGroups.CollectionChanged += (s, e) =>
                {
                    OnPropertyChanged(nameof(SelectedText));
                    UpdateSelectionCountText();
                };
            }
            
            RefreshSelectionItems();
            OnPropertyChanged(nameof(SelectedText));
            UpdateSelectionCountText();
        }

        private void OnAvailableBlockGroupsChanged()
        {
            RefreshSelectionItems();
        }

        private void RefreshSelectionItems()
        {
            _selectionItems.Clear();

            if (AvailableBlockGroups == null || SelectedBlockGroups == null)
                return;

            foreach (var blockGroup in AvailableBlockGroups)
            {
                bool isSelected = SelectedBlockGroups.Contains(blockGroup);
                var selectionItem = new BlockGroupSelectionItem(blockGroup, SelectedBlockGroups, isSelected);
                
                // Subscribe to property changes to update the UI
                selectionItem.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(BlockGroupSelectionItem.IsSelected))
                    {
                        OnPropertyChanged(nameof(SelectedText));
                        UpdateSelectionCountText();
                    }
                };
                
                _selectionItems.Add(selectionItem);
            }
        }

        private void UpdateSelectionCountText()
        {
            if (SelectionCountText != null)
            {
                var selectedCount = SelectedBlockGroups?.Count ?? 0;
                var totalCount = AvailableBlockGroups?.Count ?? 0;
                
                if (selectedCount == 0)
                    SelectionCountText.Text = "No groups selected";
                else if (selectedCount == 1)
                    SelectionCountText.Text = "1 group selected";
                else
                    SelectionCountText.Text = $"{selectedCount} of {totalCount} groups selected";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Event Handlers

        private void BlockGroupSelector_Loaded(object sender, RoutedEventArgs e)
        {
            // Handle popup opened event
            BlockGroupPopup.Opened += BlockGroupPopup_Opened;
        }

        private void BlockGroupPopup_Opened(object sender, EventArgs e)
        {
            // When popup opens, set focus to maintain edit mode
            this.Focus();
            
            // Ensure the popup stays open
            BlockGroupPopup.StaysOpen = true;
        }
        
        private void PopupBorder_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Simply maintain focus on this control to prevent DataGrid from ending edit mode
            // but don't handle the event so that all controls work normally
            this.Focus();
        }


        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableBlockGroups != null && SelectedBlockGroups != null)
            {
                foreach (var blockGroup in AvailableBlockGroups)
                {
                    if (!SelectedBlockGroups.Contains(blockGroup))
                    {
                        SelectedBlockGroups.Add(blockGroup);
                    }
                }
                
                // Update selection items
                foreach (var item in _selectionItems)
                {
                    item.IsSelected = true;
                }
            }
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedBlockGroups?.Clear();
            
            // Update selection items
            foreach (var item in _selectionItems)
            {
                item.IsSelected = false;
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            DropDownButton.IsChecked = false;
            BlockGroupPopup.IsOpen = false;
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape && BlockGroupPopup.IsOpen)
            {
                ClosePopup();
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        #endregion
    }
}