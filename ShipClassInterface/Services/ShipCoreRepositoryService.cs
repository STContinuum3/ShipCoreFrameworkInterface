using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ShipClassInterface.Models;

namespace ShipClassInterface.Services
{
    public class ShipCoreRepositoryService : INotifyPropertyChanged
    {
        private static ShipCoreRepositoryService? _instance;
        public static ShipCoreRepositoryService Instance => _instance ??= new ShipCoreRepositoryService();

        private ObservableCollection<ShipCore> _availableShipCores = new();
        
        public ObservableCollection<ShipCore> AvailableShipCores
        {
            get => _availableShipCores;
            private set
            {
                _availableShipCores = value;
                OnPropertyChanged();
            }
        }

        private ShipCoreRepositoryService()
        {
        }

        public void AddShipCore(ShipCore shipCore)
        {
            // Check if ship core with same SubtypeId already exists
            var existing = AvailableShipCores.FirstOrDefault(sc => sc.SubtypeId == shipCore.SubtypeId);
            if (existing != null)
            {
                // Replace with updated version
                var index = AvailableShipCores.IndexOf(existing);
                AvailableShipCores[index] = shipCore;
            }
            else
            {
                AvailableShipCores.Add(shipCore);
            }
        }

        public void AddShipCores(IEnumerable<ShipCore> shipCores)
        {
            foreach (var shipCore in shipCores)
            {
                AddShipCore(shipCore);
            }
        }

        public void Clear()
        {
            AvailableShipCores.Clear();
        }

        public ShipCore? GetShipCoreBySubtypeId(string subtypeId)
        {
            return AvailableShipCores.FirstOrDefault(sc => sc.SubtypeId == subtypeId);
        }

        public IEnumerable<ShipCore> GetAllShipCores()
        {
            return AvailableShipCores;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}