using ShipClassInterface.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ShipClassInterface.Views
{
    public partial class BlockCreatorView : UserControl
    {
        private bool _isLoadingBlock = false;
        
        public BlockCreatorView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }
        
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is BlockCreatorViewModel oldViewModel)
            {
                oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
            
            if (e.NewValue is BlockCreatorViewModel newViewModel)
            {
                newViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }
        
        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // When SelectedBlock changes, we're loading a new block, so don't mark as changed
            if (e.PropertyName == nameof(BlockCreatorViewModel.SelectedBlock))
            {
                _isLoadingBlock = true;
                // Reset flag after a brief delay to allow bindings to update
                Dispatcher.BeginInvoke(new System.Action(() => _isLoadingBlock = false), 
                    System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            }
        }
        
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Don't mark as changed if we're in the process of loading a block
            if (_isLoadingBlock)
                return;
                
            // Mark the ViewModel as having unsaved changes when any text field is modified
            if (DataContext is BlockCreatorViewModel viewModel && viewModel.SelectedBlock != null)
            {
                viewModel.HasUnsavedChanges = true;
            }
        }
    }
}