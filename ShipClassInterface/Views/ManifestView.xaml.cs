using System.Windows.Controls;
using ShipClassInterface.ViewModels;

namespace ShipClassInterface.Views
{
    public partial class ManifestView : UserControl
    {
        public ManifestView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void TextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox && 
                DataContext is ManifestViewModel viewModel)
            {
                // Get the original value from the Tag property
                var originalValue = textBox.Tag as string;
                var newValue = textBox.Text;

                if (originalValue != null && originalValue != newValue)
                {
                    // Find and replace the item in the collection
                    var index = viewModel.Manifest.ShipCoreFiles.IndexOf(originalValue);
                    if (index >= 0)
                    {
                        viewModel.Manifest.ShipCoreFiles[index] = newValue;
                        
                        // Update the Tag to reflect the new value
                        textBox.Tag = newValue;
                        
                        // Auto-save if there's a file path
                        if (!string.IsNullOrEmpty(viewModel.CurrentFilePath))
                        {
                            viewModel.SaveConfigurationCommand.Execute(null);
                        }
                    }
                }
            }
        }
    }
}