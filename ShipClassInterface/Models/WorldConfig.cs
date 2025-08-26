using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace ShipClassInterface.Models
{
    [XmlRoot("ModConfig", Namespace = "")]
    public class WorldConfig
    {
        
        public bool DebugMode { get; set; }
        public bool CombatLogging { get; set; }
        public int LOG_LEVEL { get; set; } = 1;
        public int CLIENT_OUTPUT_LOG_LEVEL { get; set; } = 1;
        public float MaxPossibleSpeedMetersPerSecond { get; set; } = 300f;
        public bool IncludeAiFactions { get; set; }
        
        [XmlArray("IgnoreFactionTags")]
        [XmlArrayItem("string")]
        public ObservableCollection<string> IgnoreFactionTags { get; set; } = new();
        
        [XmlElement("NoFlyZones")]
        public ObservableCollection<NoFlyZone> NoFlyZones { get; set; } = new();
    }

    public class NoFlyZone
    {
        public int ID { get; set; }
        public Position Position { get; set; } = new();
        public double Radius { get; set; } = 1000;
        
        [XmlArray("AllowedCoresSubtype")]
        [XmlArrayItem("string")]
        public ObservableCollection<string> AllowedCoresSubtype { get; set; } = new();
        
        public bool OverideBlockLimitsForceShutOff { get; set; }
    }

    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}