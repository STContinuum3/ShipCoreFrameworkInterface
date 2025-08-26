using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace ShipClassInterface.Models.BlockCreator
{
    public class CubeBlockDefinition : INotifyPropertyChanged
    {
        private string _xsiType = "MyObjectBuilder_CubeBlockDefinition";
        private BlockId _id = new();
        private string _displayName = string.Empty;
        private string _description = string.Empty;
        private string _icon = string.Empty;
        private string _cubeSize = "Large";
        private string _blockTopology = "TriangleMesh";
        private BlockSize _size = new();
        private ModelOffset _modelOffset = new();
        private string _model = string.Empty;

        [XmlAttribute("type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string XsiType
        {
            get => _xsiType;
            set { if (_xsiType != value) { _xsiType = value; OnPropertyChanged(); } }
        }

        [XmlElement("Id")]
        public BlockId Id
        {
            get => _id;
            set { if (_id != value) { _id = value; OnPropertyChanged(); } }
        }

        [XmlElement("DisplayName")]
        public string DisplayName
        {
            get => _displayName;
            set { if (_displayName != value) { _displayName = value; OnPropertyChanged(); } }
        }

        [XmlElement("Description")]
        public string Description
        {
            get => _description;
            set { if (_description != value) { _description = value; OnPropertyChanged(); } }
        }

        [XmlElement("Icon")]
        public string Icon
        {
            get => _icon;
            set { if (_icon != value) { _icon = value; OnPropertyChanged(); } }
        }

        [XmlElement("CubeSize")]
        public string CubeSize
        {
            get => _cubeSize;
            set { if (_cubeSize != value) { _cubeSize = value; OnPropertyChanged(); } }
        }

        [XmlElement("BlockTopology")]
        public string BlockTopology
        {
            get => _blockTopology;
            set { if (_blockTopology != value) { _blockTopology = value; OnPropertyChanged(); } }
        }

        [XmlElement("Size")]
        public BlockSize Size
        {
            get => _size;
            set { if (_size != value) { _size = value; OnPropertyChanged(); } }
        }

        [XmlElement("ModelOffset")]
        public ModelOffset ModelOffset
        {
            get => _modelOffset;
            set { if (_modelOffset != value) { _modelOffset = value; OnPropertyChanged(); } }
        }

        [XmlElement("Model")]
        public string Model
        {
            get => _model;
            set { if (_model != value) { _model = value; OnPropertyChanged(); } }
        }

        [XmlArray("Components")]
        [XmlArrayItem("Component")]
        public ObservableCollection<Component> Components { get; set; } = new();

        [XmlElement("CriticalComponent")]
        public CriticalComponent CriticalComponent { get; set; } = new();

        [XmlArray("MountPoints")]
        [XmlArrayItem("MountPoint")]
        public ObservableCollection<MountPoint> MountPoints { get; set; } = new();

        [XmlArray("BuildProgressModels")]
        [XmlArrayItem("Model")]
        public ObservableCollection<BuildProgressModel> BuildProgressModels { get; set; } = new();

        [XmlArray("ScreenAreas")]
        [XmlArrayItem("ScreenArea")]
        public ObservableCollection<ScreenArea> ScreenAreas { get; set; } = new();

        private string _blockPairName = string.Empty;
        private string _mirroringX = string.Empty;
        private string _mirroringY = string.Empty;
        private string _mirroringZ = string.Empty;
        private string _edgeType = string.Empty;
        private int _buildTimeSeconds;

        [XmlElement("BlockPairName")]
        public string BlockPairName
        {
            get => _blockPairName;
            set { if (_blockPairName != value) { _blockPairName = value; OnPropertyChanged(); } }
        }

        [XmlElement("MirroringX")]
        public string MirroringX
        {
            get => _mirroringX;
            set { if (_mirroringX != value) { _mirroringX = value; OnPropertyChanged(); } }
        }

        [XmlElement("MirroringY")]
        public string MirroringY
        {
            get => _mirroringY;
            set { if (_mirroringY != value) { _mirroringY = value; OnPropertyChanged(); } }
        }

        [XmlElement("MirroringZ")]
        public string MirroringZ
        {
            get => _mirroringZ;
            set { if (_mirroringZ != value) { _mirroringZ = value; OnPropertyChanged(); } }
        }

        [XmlElement("EdgeType")]
        public string EdgeType
        {
            get => _edgeType;
            set { if (_edgeType != value) { _edgeType = value; OnPropertyChanged(); } }
        }

        [XmlElement("BuildTimeSeconds")]
        public int BuildTimeSeconds
        {
            get => _buildTimeSeconds;
            set { if (_buildTimeSeconds != value) { _buildTimeSeconds = value; OnPropertyChanged(); } }
        }

        [XmlElement("DamageEffectName")]
        public string DamageEffectName { get; set; } = string.Empty;

        [XmlElement("DamagedSound")]
        public string DamagedSound { get; set; } = string.Empty;

        [XmlElement("PrimarySound")]
        public string PrimarySound { get; set; } = string.Empty;

        [XmlElement("ActionSound")]
        public string ActionSound { get; set; } = string.Empty;

        [XmlElement("EmissiveColorPreset")]
        public string EmissiveColorPreset { get; set; } = string.Empty;

        [XmlElement("DestroyEffect")]
        public string DestroyEffect { get; set; } = string.Empty;

        [XmlElement("DestroySound")]
        public string DestroySound { get; set; } = string.Empty;

        [XmlElement("PCU")]
        public int PCU { get; set; }

        [XmlArray("TieredUpdateTimes")]
        [XmlArrayItem("unsignedInt")]
        public ObservableCollection<int> TieredUpdateTimes { get; set; } = new();

        // Additional optional properties from SE wiki
        [XmlElement("IsAirTight")]
        public bool? IsAirTight { get; set; }

        [XmlElement("UseNeighbourOxygenRooms")]
        public bool? UseNeighbourOxygenRooms { get; set; }

        [XmlElement("GeneralDamageMultiplier")]
        public double? GeneralDamageMultiplier { get; set; }

        [XmlElement("DamageThreshold")]
        public int? DamageThreshold { get; set; }

        [XmlElement("UsesDeformation")]
        public bool? UsesDeformation { get; set; }

        [XmlElement("DeformationRatio")]
        public double? DeformationRatio { get; set; }

        [XmlElement("SilenceableByShipSoundSystem")]
        public bool? SilenceableByShipSoundSystem { get; set; }

        [XmlElement("Points")]
        public int? Points { get; set; }

        [XmlElement("UseModelIntersection")]
        public bool? UseModelIntersection { get; set; }

        [XmlElement("NavigationDefinition")]
        public string NavigationDefinition { get; set; } = string.Empty;

        [XmlElement("PhysicalMaterial")]
        public string PhysicalMaterial { get; set; } = string.Empty;

        [XmlElement("MirroringBlock")]
        public string MirroringBlock { get; set; } = string.Empty;

        [XmlElement("MultiBlock")]
        public string MultiBlock { get; set; } = string.Empty;

        [XmlElement("GuiVisible")]
        public bool? GuiVisible { get; set; }

        [XmlElement("VisibleInSurvival")]
        public bool? VisibleInSurvival { get; set; }

        [XmlElement("BlockVariants")]
        public string BlockVariants { get; set; } = string.Empty;

        [XmlElement("CompoundEnabled")]
        public bool? CompoundEnabled { get; set; }

        [XmlElement("CreateFracturedPieces")]
        public bool? CreateFracturedPieces { get; set; }

        [XmlElement("RandomRotation")]
        public bool? RandomRotation { get; set; }

        [XmlElement("Public")]
        public bool? Public { get; set; }

        [XmlElement("AvailableInSurvival")]
        public bool? AvailableInSurvival { get; set; }

        [XmlElement("Center")]
        public Vector3? Center { get; set; }

        [XmlElement("BuildType")]
        public string BuildType { get; set; } = string.Empty;

        [XmlElement("BuildMaterial")]
        public string BuildMaterial { get; set; } = string.Empty;

        [XmlElement("GeneratedBlockType")]
        public string GeneratedBlockType { get; set; } = string.Empty;

        [XmlElement("DamageEffectId")]
        public int? DamageEffectId { get; set; }

        [XmlElement("CompoundMaterial")]
        public string CompoundMaterial { get; set; } = string.Empty;

        [XmlElement("PlaceDecals")]
        public bool? PlaceDecals { get; set; }

        [XmlArray("VoxelPlacement")]
        [XmlArrayItem("StaticMode")]
        public ObservableCollection<VoxelPlacement> VoxelPlacements { get; set; } = new();

        [XmlElement("ShadowCastingMode")]
        public string ShadowCastingMode { get; set; } = string.Empty;

        [XmlElement("EnableUseObjectSimpleTargeting")]
        public bool? EnableUseObjectSimpleTargeting { get; set; }

        [XmlElement("NewsletterSubscriptionNeeded")]
        public bool? NewsletterSubscriptionNeeded { get; set; }

        [XmlElement("YesNoToolbarBackground")]
        public bool? YesNoToolbarBackground { get; set; }

        [XmlElement("YesNoToolbarYesTooltip")]
        public string YesNoToolbarYesTooltip { get; set; } = string.Empty;

        [XmlElement("YesNoToolbarNoTooltip")]
        public string YesNoToolbarNoTooltip { get; set; } = string.Empty;

        [XmlElement("AllowInteractionThroughBlock")]
        public bool? AllowInteractionThroughBlock { get; set; }

        [XmlElement("WheelPlacementCollider")]
        public WheelPlacementCollider? WheelPlacementCollider { get; set; }

        [XmlArray("Upgrades")]
        [XmlArrayItem("MyUpgradeModuleInfo")]
        public ObservableCollection<UpgradeModuleInfo> Upgrades { get; set; } = new();

        // Block Category specific properties
        [XmlElement("SearchBlocks")]
        public bool? SearchBlocks { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("IsToolCategory")]
        public bool? IsToolCategory { get; set; }

        [XmlArray("ItemIds")]
        [XmlArrayItem("string")]
        public ObservableCollection<string> ItemIds { get; set; } = new();

        // Track which file this block came from (not serialized to XML)
        [XmlIgnore]
        public string? SourceFilePath { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CubeBlockDefinition Clone()
        {
            var cloned = new CubeBlockDefinition
            {
                XsiType = this.XsiType,
                Id = new BlockId { TypeId = this.Id.TypeId, SubtypeId = this.Id.SubtypeId },
                DisplayName = this.DisplayName,
                Description = this.Description,
                Icon = this.Icon,
                CubeSize = this.CubeSize,
                BlockTopology = this.BlockTopology,
                Size = new BlockSize { X = this.Size.X, Y = this.Size.Y, Z = this.Size.Z },
                ModelOffset = new ModelOffset { X = this.ModelOffset.X, Y = this.ModelOffset.Y, Z = this.ModelOffset.Z },
                Model = this.Model,
                BlockPairName = this.BlockPairName,
                MirroringX = this.MirroringX,
                MirroringY = this.MirroringY,
                MirroringZ = this.MirroringZ,
                EdgeType = this.EdgeType,
                BuildTimeSeconds = this.BuildTimeSeconds,
                DamageEffectName = this.DamageEffectName,
                DamagedSound = this.DamagedSound,
                PrimarySound = this.PrimarySound,
                ActionSound = this.ActionSound,
                EmissiveColorPreset = this.EmissiveColorPreset,
                DestroyEffect = this.DestroyEffect,
                DestroySound = this.DestroySound,
                PCU = this.PCU,
                IsAirTight = this.IsAirTight,
                UseNeighbourOxygenRooms = this.UseNeighbourOxygenRooms,
                GeneralDamageMultiplier = this.GeneralDamageMultiplier,
                DamageThreshold = this.DamageThreshold,
                UsesDeformation = this.UsesDeformation,
                DeformationRatio = this.DeformationRatio,
                SilenceableByShipSoundSystem = this.SilenceableByShipSoundSystem,
                Points = this.Points,
                UseModelIntersection = this.UseModelIntersection,
                NavigationDefinition = this.NavigationDefinition,
                PhysicalMaterial = this.PhysicalMaterial,
                MirroringBlock = this.MirroringBlock,
                MultiBlock = this.MultiBlock,
                GuiVisible = this.GuiVisible,
                VisibleInSurvival = this.VisibleInSurvival,
                BlockVariants = this.BlockVariants,
                CompoundEnabled = this.CompoundEnabled,
                CreateFracturedPieces = this.CreateFracturedPieces,
                RandomRotation = this.RandomRotation,
                Public = this.Public,
                AvailableInSurvival = this.AvailableInSurvival,
                Center = this.Center != null ? new Vector3 { X = this.Center.X, Y = this.Center.Y, Z = this.Center.Z } : null,
                BuildType = this.BuildType,
                BuildMaterial = this.BuildMaterial,
                GeneratedBlockType = this.GeneratedBlockType,
                DamageEffectId = this.DamageEffectId,
                CompoundMaterial = this.CompoundMaterial,
                PlaceDecals = this.PlaceDecals,
                ShadowCastingMode = this.ShadowCastingMode,
                EnableUseObjectSimpleTargeting = this.EnableUseObjectSimpleTargeting,
                NewsletterSubscriptionNeeded = this.NewsletterSubscriptionNeeded,
                YesNoToolbarBackground = this.YesNoToolbarBackground,
                YesNoToolbarYesTooltip = this.YesNoToolbarYesTooltip,
                YesNoToolbarNoTooltip = this.YesNoToolbarNoTooltip,
                AllowInteractionThroughBlock = this.AllowInteractionThroughBlock,
                WheelPlacementCollider = this.WheelPlacementCollider != null ? new WheelPlacementCollider 
                { 
                    ColliderDiameter = this.WheelPlacementCollider.ColliderDiameter,
                    ColliderHeight = this.WheelPlacementCollider.ColliderHeight,
                    ColliderOffset = this.WheelPlacementCollider.ColliderOffset 
                } : null,
                SearchBlocks = this.SearchBlocks,
                Name = this.Name,
                IsToolCategory = this.IsToolCategory,
                SourceFilePath = this.SourceFilePath,
                CriticalComponent = new CriticalComponent { Subtype = this.CriticalComponent.Subtype, Index = this.CriticalComponent.Index }
            };

            // Clone collections
            foreach (var component in this.Components)
            {
                cloned.Components.Add(new Component { Subtype = component.Subtype, Count = component.Count });
            }

            foreach (var mountPoint in this.MountPoints)
            {
                cloned.MountPoints.Add(new MountPoint 
                { 
                    Side = mountPoint.Side, 
                    StartX = mountPoint.StartX, 
                    StartY = mountPoint.StartY, 
                    EndX = mountPoint.EndX, 
                    EndY = mountPoint.EndY 
                });
            }

            foreach (var model in this.BuildProgressModels)
            {
                cloned.BuildProgressModels.Add(new BuildProgressModel 
                { 
                    BuildPercentUpperBound = model.BuildPercentUpperBound, 
                    File = model.File 
                });
            }

            foreach (var screenArea in this.ScreenAreas)
            {
                cloned.ScreenAreas.Add(new ScreenArea 
                { 
                    Name = screenArea.Name, 
                    DisplayName = screenArea.DisplayName, 
                    TextureResolution = screenArea.TextureResolution, 
                    ScreenWidth = screenArea.ScreenWidth, 
                    ScreenHeight = screenArea.ScreenHeight 
                });
            }

            foreach (var time in this.TieredUpdateTimes)
            {
                cloned.TieredUpdateTimes.Add(time);
            }

            foreach (var placement in this.VoxelPlacements)
            {
                cloned.VoxelPlacements.Add(new VoxelPlacement 
                { 
                    PlacementMode = placement.PlacementMode, 
                    MinAllowed = placement.MinAllowed, 
                    MaxAllowed = placement.MaxAllowed 
                });
            }

            foreach (var upgrade in this.Upgrades)
            {
                cloned.Upgrades.Add(new UpgradeModuleInfo 
                { 
                    UpgradeType = upgrade.UpgradeType, 
                    Modifier = upgrade.Modifier, 
                    ModifierType = upgrade.ModifierType 
                });
            }

            foreach (var itemId in this.ItemIds)
            {
                cloned.ItemIds.Add(itemId);
            }

            return cloned;
        }
    }

    public class Vector3
    {
        [XmlAttribute("x")]
        public double X { get; set; }

        [XmlAttribute("y")]
        public double Y { get; set; }

        [XmlAttribute("z")]
        public double Z { get; set; }
    }

    public class VoxelPlacement
    {
        [XmlElement("PlacementMode")]
        public string PlacementMode { get; set; } = string.Empty;

        [XmlElement("MinAllowed")]
        public double MinAllowed { get; set; }

        [XmlElement("MaxAllowed")]
        public double MaxAllowed { get; set; }
    }

    public class WheelPlacementCollider
    {
        [XmlElement("ColliderDiameter")]
        public double ColliderDiameter { get; set; }

        [XmlElement("ColliderHeight")]
        public double ColliderHeight { get; set; }

        [XmlElement("ColliderOffset")]
        public double ColliderOffset { get; set; }
    }

    public class UpgradeModuleInfo
    {
        [XmlElement("UpgradeType")]
        public string UpgradeType { get; set; } = string.Empty;

        [XmlElement("Modifier")]
        public double Modifier { get; set; }

        [XmlElement("ModifierType")]
        public string ModifierType { get; set; } = "Additive";
    }

    public class BlockId : INotifyPropertyChanged
    {
        private string _typeId = "TerminalBlock";
        private string _subtypeId = string.Empty;

        [XmlElement("TypeId")]
        public string TypeId
        {
            get => _typeId;
            set
            {
                if (_typeId != value)
                {
                    _typeId = value;
                    OnPropertyChanged();
                }
            }
        }

        [XmlElement("SubtypeId")]
        public string SubtypeId
        {
            get => _subtypeId;
            set
            {
                if (_subtypeId != value)
                {
                    _subtypeId = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BlockSize : INotifyPropertyChanged
    {
        private int _x = 1;
        private int _y = 1;
        private int _z = 1;

        [XmlAttribute("x")]
        public int X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged();
                }
            }
        }

        [XmlAttribute("y")]
        public int Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged();
                }
            }
        }

        [XmlAttribute("z")]
        public int Z
        {
            get => _z;
            set
            {
                if (_z != value)
                {
                    _z = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ModelOffset : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private double _z;

        [XmlAttribute("x")]
        public double X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged();
                }
            }
        }

        [XmlAttribute("y")]
        public double Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged();
                }
            }
        }

        [XmlAttribute("z")]
        public double Z
        {
            get => _z;
            set
            {
                if (_z != value)
                {
                    _z = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Component
    {
        [XmlAttribute("Subtype")]
        public string Subtype { get; set; } = string.Empty;

        [XmlAttribute("Count")]
        public int Count { get; set; }
    }

    public class CriticalComponent
    {
        [XmlAttribute("Subtype")]
        public string Subtype { get; set; } = string.Empty;

        [XmlAttribute("Index")]
        public int Index { get; set; }
    }

    public class MountPoint
    {
        [XmlAttribute("Side")]
        public string Side { get; set; } = string.Empty;

        [XmlAttribute("StartX")]
        public double StartX { get; set; }

        [XmlAttribute("StartY")]
        public double StartY { get; set; }

        [XmlAttribute("EndX")]
        public double EndX { get; set; }

        [XmlAttribute("EndY")]
        public double EndY { get; set; }
    }

    public class BuildProgressModel
    {
        [XmlAttribute("BuildPercentUpperBound")]
        public double BuildPercentUpperBound { get; set; }

        [XmlAttribute("File")]
        public string File { get; set; } = string.Empty;
    }

    public class ScreenArea
    {
        [XmlAttribute("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute("DisplayName")]
        public string DisplayName { get; set; } = string.Empty;

        [XmlAttribute("TextureResolution")]
        public int TextureResolution { get; set; } = 512;

        [XmlAttribute("ScreenWidth")]
        public int ScreenWidth { get; set; } = 5;

        [XmlAttribute("ScreenHeight")]
        public int ScreenHeight { get; set; } = 3;
    }
}