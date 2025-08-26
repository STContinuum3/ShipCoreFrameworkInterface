using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace ShipClassInterface.Models
{
    [XmlRoot("ArrayOfBlockGroup", Namespace = "")]
    public class BlockGroupCollection
    {
        [XmlElement("BlockGroup")]
        public ObservableCollection<BlockGroup> BlockGroups { get; set; } = new();
    }

    public class BlockGroup
    {
        public string Name { get; set; } = string.Empty;
        
        [XmlElement("BlockTypes")]
        public ObservableCollection<BlockType> BlockTypes { get; set; } = new();
    }

    public class BlockType
    {
        public string TypeId { get; set; } = string.Empty;
        public string SubtypeId { get; set; } = "any";
        public float CountWeight { get; set; } = 1.0f;
    }
}