using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace ShipClassInterface.Models.BlockCreator
{
    [XmlRoot("Definitions")]
    public class BlockDefinitions
    {
        [XmlAttribute("xmlns:xsi")]
        public string XmlnsXsi { get; set; } = "http://www.w3.org/2001/XMLSchema-instance";

        [XmlAttribute("xmlns:xsd")]
        public string XmlnsXsd { get; set; } = "http://www.w3.org/2001/XMLSchema";

        [XmlArray("CubeBlocks")]
        [XmlArrayItem("Definition")]
        public ObservableCollection<CubeBlockDefinition> CubeBlocks { get; set; } = new();
    }
}