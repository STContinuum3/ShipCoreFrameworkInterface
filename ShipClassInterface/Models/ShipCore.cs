using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace ShipClassInterface.Models
{
    [XmlRoot("ShipCore", Namespace = "")]
    public class ShipCore : INotifyPropertyChanged
    {
        private string _subtypeId = string.Empty;
        private string _uniqueName = string.Empty;
        
        // Not serialized - used internally to track the source file
        [XmlIgnore]
        public string? SourceFilePath { get; set; }
        
        public string SubtypeId 
        { 
            get => _subtypeId; 
            set 
            { 
                _subtypeId = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public string UniqueName 
        { 
            get => _uniqueName; 
            set 
            { 
                _uniqueName = value; 
                OnPropertyChanged(); 
            } 
        }
        private bool _forceBroadCast;
        private float _forceBroadCastRange = 10000f;
        
        public bool ForceBroadCast 
        { 
            get => _forceBroadCast; 
            set 
            { 
                _forceBroadCast = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public float ForceBroadCastRange 
        { 
            get => _forceBroadCastRange; 
            set 
            { 
                _forceBroadCastRange = value; 
                OnPropertyChanged(); 
            } 
        }
        
        private bool _largeGridStatic = true;
        private bool _largeGridMobile = true;
        
        public bool LargeGridStatic 
        { 
            get => _largeGridStatic; 
            set 
            { 
                _largeGridStatic = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public bool LargeGridMobile 
        { 
            get => _largeGridMobile; 
            set 
            { 
                _largeGridMobile = value; 
                OnPropertyChanged(); 
            } 
        }
        
        private int _maxBlocks = -1;
        private float _maxMass = -1;
        private int _maxPCU = -1;
        private int _maxPerFaction = -1;
        private int _maxPerPlayer = -1;
        private int _minBlocks = -1;
        private int _minPlayers = -1;
        
        public int MaxBlocks 
        { 
            get => _maxBlocks; 
            set 
            { 
                _maxBlocks = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public float MaxMass 
        { 
            get => _maxMass; 
            set 
            { 
                _maxMass = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public int MaxPCU 
        { 
            get => _maxPCU; 
            set 
            { 
                _maxPCU = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public int MaxPerFaction 
        { 
            get => _maxPerFaction; 
            set 
            { 
                _maxPerFaction = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public int MaxPerPlayer 
        { 
            get => _maxPerPlayer; 
            set 
            { 
                _maxPerPlayer = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public int MinBlocks 
        { 
            get => _minBlocks; 
            set 
            { 
                _minBlocks = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public int MinPlayers 
        { 
            get => _minPlayers; 
            set 
            { 
                _minPlayers = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public ShipModifiers Modifiers { get; set; } = new();
        public DefenseModifiers PassiveDefenseModifiers { get; set; } = new();
        private bool _enableActiveDefenseModifiers;
        
        public bool EnableActiveDefenseModifiers 
        { 
            get => _enableActiveDefenseModifiers; 
            set 
            { 
                _enableActiveDefenseModifiers = value; 
                OnPropertyChanged(); 
            } 
        }
        public DefenseModifiers ActiveDefenseModifiers { get; set; } = new();
        
        private bool _speedBoostEnabled;
        private bool _enableReloadModifier;
        private float _reloadModifier = 1.0f;
        
        public bool SpeedBoostEnabled 
        { 
            get => _speedBoostEnabled; 
            set 
            { 
                _speedBoostEnabled = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public bool EnableReloadModifier 
        { 
            get => _enableReloadModifier; 
            set 
            { 
                _enableReloadModifier = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public float ReloadModifier 
        { 
            get => _reloadModifier; 
            set 
            { 
                _reloadModifier = value; 
                OnPropertyChanged(); 
            } 
        }
        
        [XmlElement("BlockLimits")]
        public ObservableCollection<BlockLimit> BlockLimits { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ShipModifiers
    {
        private float _maxSpeed = 1.0f;
        
        public float AssemblerSpeed { get; set; } = 1.0f;
        public float DrillHarvestMultiplier { get; set; } = 1.0f;
        public float GyroEfficiency { get; set; } = 1.0f;
        public float GyroForce { get; set; } = 1.0f;
        public float PowerProducersOutput { get; set; } = 1.0f;
        public float RefineEfficiency { get; set; } = 1.0f;
        public float RefineSpeed { get; set; } = 1.0f;
        public float ThrusterEfficiency { get; set; } = 1.0f;
        public float ThrusterForce { get; set; } = 1.0f;
        
        public float MaxSpeed 
        { 
            get => _maxSpeed;
            set 
            {
                // Enforce maximum value of 1.0
                _maxSpeed = value > 1.0f ? 1.0f : (value < 0 ? 0 : value);
            }
        }
        
        public float MaxBoost { get; set; } = 1.5f;
        public float BoostDuration { get; set; } = 10f;
        public float BoostCoolDown { get; set; } = 30f;
    }

    public class DefenseModifiers
    {
        public float Bullet { get; set; } = 1.0f;
        public float Energy { get; set; } = 1.0f;
        public float Kinetic { get; set; } = 1.0f;
        public float Duration { get; set; } = 10f;
        public float Cooldown { get; set; } = 60f;
        public float Rocket { get; set; } = 1.0f;
        public float Explosion { get; set; } = 1.0f;
        public float Environment { get; set; } = 1.0f;
    }

    public class BlockLimit : INotifyPropertyChanged
    {
        public string Name { get; set; } = string.Empty;
        public string BlockGroups { get; set; } = string.Empty;
        public float MaxCount { get; set; } = 1;
        public bool TurnedOffByNoFlyZone { get; set; }
        public PunishmentType PunishmentType { get; set; } = PunishmentType.ShutOff;
        
        [XmlElement("AllowedDirections")]
        public ObservableCollection<string> AllowedDirections { get; set; } = new();
        
        [XmlIgnore]
        public string AllowedDirectionsText
        {
            get => string.Join(", ", AllowedDirections);
            set
            {
                AllowedDirections.Clear();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var directions = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(d => d.Trim())
                                          .Where(d => !string.IsNullOrEmpty(d));
                    foreach (var direction in directions)
                    {
                        AllowedDirections.Add(direction);
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(ForwardSelected));
                OnPropertyChanged(nameof(BackwardSelected));
                OnPropertyChanged(nameof(UpSelected));
                OnPropertyChanged(nameof(DownSelected));
                OnPropertyChanged(nameof(LeftSelected));
                OnPropertyChanged(nameof(RightSelected));
            }
        }

        [XmlIgnore]
        public bool ForwardSelected
        {
            get => AllowedDirections.Contains("Forward");
            set
            {
                if (value && !AllowedDirections.Contains("Forward"))
                    AllowedDirections.Add("Forward");
                else if (!value && AllowedDirections.Contains("Forward"))
                    AllowedDirections.Remove("Forward");
                OnPropertyChanged();
                OnPropertyChanged(nameof(AllowedDirectionsText));
            }
        }

        [XmlIgnore]
        public bool BackwardSelected
        {
            get => AllowedDirections.Contains("Backward");
            set
            {
                if (value && !AllowedDirections.Contains("Backward"))
                    AllowedDirections.Add("Backward");
                else if (!value && AllowedDirections.Contains("Backward"))
                    AllowedDirections.Remove("Backward");
                OnPropertyChanged();
                OnPropertyChanged(nameof(AllowedDirectionsText));
            }
        }

        [XmlIgnore]
        public bool UpSelected
        {
            get => AllowedDirections.Contains("Up");
            set
            {
                if (value && !AllowedDirections.Contains("Up"))
                    AllowedDirections.Add("Up");
                else if (!value && AllowedDirections.Contains("Up"))
                    AllowedDirections.Remove("Up");
                OnPropertyChanged();
                OnPropertyChanged(nameof(AllowedDirectionsText));
            }
        }

        [XmlIgnore]
        public bool DownSelected
        {
            get => AllowedDirections.Contains("Down");
            set
            {
                if (value && !AllowedDirections.Contains("Down"))
                    AllowedDirections.Add("Down");
                else if (!value && AllowedDirections.Contains("Down"))
                    AllowedDirections.Remove("Down");
                OnPropertyChanged();
                OnPropertyChanged(nameof(AllowedDirectionsText));
            }
        }

        [XmlIgnore]
        public bool LeftSelected
        {
            get => AllowedDirections.Contains("Left");
            set
            {
                if (value && !AllowedDirections.Contains("Left"))
                    AllowedDirections.Add("Left");
                else if (!value && AllowedDirections.Contains("Left"))
                    AllowedDirections.Remove("Left");
                OnPropertyChanged();
                OnPropertyChanged(nameof(AllowedDirectionsText));
            }
        }

        [XmlIgnore]
        public bool RightSelected
        {
            get => AllowedDirections.Contains("Right");
            set
            {
                if (value && !AllowedDirections.Contains("Right"))
                    AllowedDirections.Add("Right");
                else if (!value && AllowedDirections.Contains("Right"))
                    AllowedDirections.Remove("Right");
                OnPropertyChanged();
                OnPropertyChanged(nameof(AllowedDirectionsText));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum PunishmentType
    {
        ShutOff,
        Damage,
        Delete,
        Explode
    }

    public enum DirectionType
    {
        Forward,
        Backward,
        Up,
        Down,
        Left,
        Right,
        Any
    }
}