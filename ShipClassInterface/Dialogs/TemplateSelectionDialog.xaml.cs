using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace ShipClassInterface.Dialogs
{
    public partial class TemplateSelectionDialog : Window
    {
        public string SelectedTemplatePath { get; private set; } = string.Empty;
        public string CustomTemplatePath { get; set; } = string.Empty;

        private readonly string _blockCategoriesPath = @"C:\SE-Projects\SE-Ship_Class_System\ArcaneShipCores\src\Data\BlockCategories.sbc";
        private readonly string _gridCorePath = @"C:\SE-Projects\SE-Ship_Class_System\ArcaneShipCores\src\Data\GridCore.sbc";
        private readonly string _upgradeModulePath = @"C:\SE-Projects\SE-Ship_Class_System\ArcaneShipCores\src\Data\GridCore_UpgradeModule.sbc";

        public TemplateSelectionDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void LoadTemplate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BlockCategoriesRadio.IsChecked == true)
                {
                    if (!File.Exists(_blockCategoriesPath))
                    {
                        MessageBox.Show($"Block Categories template file not found at:\n{_blockCategoriesPath}", 
                            "Template Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    SelectedTemplatePath = _blockCategoriesPath;
                }
                else if (GridCoreRadio.IsChecked == true)
                {
                    if (!File.Exists(_gridCorePath))
                    {
                        MessageBox.Show($"Grid Core template file not found at:\n{_gridCorePath}", 
                            "Template Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    SelectedTemplatePath = _gridCorePath;
                }
                else if (UpgradeModuleRadio.IsChecked == true)
                {
                    if (!File.Exists(_upgradeModulePath))
                    {
                        MessageBox.Show($"Upgrade Module template file not found at:\n{_upgradeModulePath}", 
                            "Template Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    SelectedTemplatePath = _upgradeModulePath;
                }
                else if (CustomPathRadio.IsChecked == true)
                {
                    if (string.IsNullOrWhiteSpace(CustomTemplatePath))
                    {
                        MessageBox.Show("Please select a custom template file.", 
                            "No File Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (!File.Exists(CustomTemplatePath))
                    {
                        MessageBox.Show($"Custom template file not found at:\n{CustomTemplatePath}", 
                            "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    SelectedTemplatePath = CustomTemplatePath;
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading template: {ex.Message}", 
                    "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "SBC files (*.sbc)|*.sbc|All files (*.*)|*.*",
                Title = "Select Custom Template File",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                CustomTemplatePath = openFileDialog.FileName;
                CustomPathTextBox.Text = CustomTemplatePath;
            }
        }
    }
}