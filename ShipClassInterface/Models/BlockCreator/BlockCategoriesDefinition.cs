using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace ShipClassInterface.Models.BlockCreator
{
    [XmlRoot("Definitions")]
    public class BlockCategoriesDefinition
    {
        [XmlAttribute("xmlns:xsi")]
        public string XmlnsXsi { get; set; } = "http://www.w3.org/2001/XMLSchema-instance";

        [XmlAttribute("xmlns:xsd")]
        public string XmlnsXsd { get; set; } = "http://www.w3.org/2001/XMLSchema";

        [XmlArray("CategoryClasses")]
        [XmlArrayItem("Category")]
        public ObservableCollection<CategoryDefinition> Categories { get; set; } = new();
    }

    public class CategoryDefinition
    {
        [XmlAttribute("type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string XsiType { get; set; } = "MyObjectBuilder_GuiBlockCategoryDefinition";

        [XmlElement("Id")]
        public CategoryId Id { get; set; } = new();

        [XmlElement("DisplayName")]
        public string DisplayName { get; set; } = string.Empty;

        [XmlElement("SearchBlocks")]
        public bool SearchBlocks { get; set; } = true;

        [XmlArray("ItemIds")]
        [XmlArrayItem("string")]
        public ObservableCollection<string> ItemIds { get; set; } = new();
    }

    public class CategoryId
    {
        [XmlElement("TypeId")]
        public string TypeId { get; set; } = "GuiBlockCategoryDefinition";

        [XmlElement("SubtypeId")]
        public string SubtypeId { get; set; } = string.Empty;
    }
}