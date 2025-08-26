using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace ShipClassInterface.Dialogs
{
    public partial class ExcelExportDialog : UserControl
    {
        public bool Result { get; private set; }

        public ExcelExportDialog()
        {
            InitializeComponent();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            DialogHost.CloseDialogCommand.Execute(true, this);
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            DialogHost.CloseDialogCommand.Execute(false, this);
        }
    }
}