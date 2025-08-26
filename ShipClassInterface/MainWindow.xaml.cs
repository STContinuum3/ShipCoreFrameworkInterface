using ShipClassInterface.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace ShipClassInterface
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true
                });
                e.Handled = true;
            }
            catch
            {
                // Silently handle any errors opening the browser
            }
        }
    }
}