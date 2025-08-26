using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using ShipClassInterface.Models;
using ShipClassInterface.Services;
using System.Collections.ObjectModel;

namespace ShipClassInterface.ViewModels
{
    public partial class ShipCoreSelectionViewModel : ObservableObject
    {
        private readonly ShipCoreRepositoryService shipCoreRepository = ShipCoreRepositoryService.Instance;

        public ObservableCollection<ShipCore> AvailableShipCores => shipCoreRepository.AvailableShipCores;

        public bool HasNoShipCores => AvailableShipCores.Count == 0;

        public ShipCore? SelectedShipCore { get; private set; }

        public ShipCoreSelectionViewModel()
        {
            shipCoreRepository.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ShipCoreRepositoryService.AvailableShipCores))
                {
                    OnPropertyChanged(nameof(AvailableShipCores));
                    OnPropertyChanged(nameof(HasNoShipCores));
                }
            };
        }

        [RelayCommand]
        private void SelectShipCore(ShipCore? shipCore)
        {
            if (shipCore != null)
            {
                SelectedShipCore = CloneShipCore(shipCore);
                DialogHost.CloseDialogCommand.Execute(true, null);
            }
        }

        [RelayCommand]
        private void CreateNew()
        {
            // Create a new ship core with default values
            SelectedShipCore = new ShipCore
            {
                SubtypeId = "New_Ship_Core",
                UniqueName = "New Ship Class",
                ForceBroadCast = false,
                ForceBroadCastRange = 0,
                LargeGridStatic = true,
                LargeGridMobile = true,
                MaxBlocks = 10000,
                MaxMass = -1,
                MaxPCU = -1,
                MaxPerFaction = -1,
                MaxPerPlayer = -1,
                MinBlocks = -1,
                MinPlayers = -1,
                Modifiers = new ShipModifiers
                {
                    AssemblerSpeed = 1.0f,
                    DrillHarvestMultiplier = 1.0f,
                    GyroEfficiency = 1.0f,
                    GyroForce = 1.0f,
                    PowerProducersOutput = 1.0f,
                    RefineEfficiency = 1.0f,
                    RefineSpeed = 1.0f,
                    ThrusterEfficiency = 1.0f,
                    ThrusterForce = 1.0f,
                    MaxSpeed = 0.5f,
                    MaxBoost = 1.0f,
                    BoostDuration = 0,
                    BoostCoolDown = 0
                },
                PassiveDefenseModifiers = new DefenseModifiers
                {
                    Bullet = 1.0f,
                    Energy = 1.0f,
                    Kinetic = 1.0f,
                    Duration = 0,
                    Cooldown = 0,
                    Rocket = 1.0f,
                    Explosion = 1.0f,
                    Environment = 1.0f
                },
                SpeedBoostEnabled = false,
                EnableActiveDefenseModifiers = false,
                ActiveDefenseModifiers = new DefenseModifiers
                {
                    Bullet = 1.0f,
                    Energy = 1.0f,
                    Kinetic = 1.0f,
                    Duration = 0,
                    Cooldown = 0,
                    Rocket = 1.0f,
                    Explosion = 1.0f,
                    Environment = 1.0f
                },
                EnableReloadModifier = false,
                ReloadModifier = 1.0f,
                BlockLimits = new ObservableCollection<BlockLimit>()
            };

            DialogHost.CloseDialogCommand.Execute(true, null);
        }

        [RelayCommand]
        private void Cancel()
        {
            SelectedShipCore = null;
            DialogHost.CloseDialogCommand.Execute(false, null);
        }

        private ShipCore CloneShipCore(ShipCore original)
        {
            // Create a deep copy of the ship core
            return new ShipCore
            {
                SubtypeId = original.SubtypeId,
                UniqueName = original.UniqueName,
                ForceBroadCast = original.ForceBroadCast,
                ForceBroadCastRange = original.ForceBroadCastRange,
                LargeGridStatic = original.LargeGridStatic,
                LargeGridMobile = original.LargeGridMobile,
                MaxBlocks = original.MaxBlocks,
                MaxMass = original.MaxMass,
                MaxPCU = original.MaxPCU,
                MaxPerFaction = original.MaxPerFaction,
                MaxPerPlayer = original.MaxPerPlayer,
                MinBlocks = original.MinBlocks,
                MinPlayers = original.MinPlayers,
                Modifiers = new ShipModifiers
                {
                    AssemblerSpeed = original.Modifiers.AssemblerSpeed,
                    DrillHarvestMultiplier = original.Modifiers.DrillHarvestMultiplier,
                    GyroEfficiency = original.Modifiers.GyroEfficiency,
                    GyroForce = original.Modifiers.GyroForce,
                    PowerProducersOutput = original.Modifiers.PowerProducersOutput,
                    RefineEfficiency = original.Modifiers.RefineEfficiency,
                    RefineSpeed = original.Modifiers.RefineSpeed,
                    ThrusterEfficiency = original.Modifiers.ThrusterEfficiency,
                    ThrusterForce = original.Modifiers.ThrusterForce,
                    MaxSpeed = original.Modifiers.MaxSpeed,
                    MaxBoost = original.Modifiers.MaxBoost,
                    BoostDuration = original.Modifiers.BoostDuration,
                    BoostCoolDown = original.Modifiers.BoostCoolDown
                },
                PassiveDefenseModifiers = CloneDefenseModifiers(original.PassiveDefenseModifiers),
                SpeedBoostEnabled = original.SpeedBoostEnabled,
                EnableActiveDefenseModifiers = original.EnableActiveDefenseModifiers,
                ActiveDefenseModifiers = CloneDefenseModifiers(original.ActiveDefenseModifiers),
                EnableReloadModifier = original.EnableReloadModifier,
                ReloadModifier = original.ReloadModifier,
                BlockLimits = new ObservableCollection<BlockLimit>(original.BlockLimits.Select(bl => new BlockLimit
                {
                    Name = bl.Name,
                    BlockGroups = bl.BlockGroups,
                    MaxCount = bl.MaxCount,
                    TurnedOffByNoFlyZone = bl.TurnedOffByNoFlyZone,
                    PunishmentType = bl.PunishmentType,
                    AllowedDirections = new ObservableCollection<string>(bl.AllowedDirections)
                }))
            };
        }

        private DefenseModifiers CloneDefenseModifiers(DefenseModifiers original)
        {
            return new DefenseModifiers
            {
                Bullet = original.Bullet,
                Energy = original.Energy,
                Kinetic = original.Kinetic,
                Duration = original.Duration,
                Cooldown = original.Cooldown,
                Rocket = original.Rocket,
                Explosion = original.Explosion,
                Environment = original.Environment
            };
        }
    }
}