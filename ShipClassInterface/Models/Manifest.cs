using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace ShipClassInterface.Models
{
    [XmlRoot("CoreManifest")]
    public class Manifest : INotifyPropertyChanged
    {
        private ObservableCollection<string> _shipCoreFiles = new();

        [XmlElement("ShipCoreFilenames")]
        public ObservableCollection<string> ShipCoreFiles
        {
            get => _shipCoreFiles;
            set
            {
                _shipCoreFiles = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}