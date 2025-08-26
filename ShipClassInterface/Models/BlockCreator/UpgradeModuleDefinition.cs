using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace ShipClassInterface.Models.BlockCreator
{
    public class UpgradeModuleDefinition : CubeBlockDefinition
    {
        public UpgradeModuleDefinition()
        {
            XsiType = "MyObjectBuilder_UpgradeModuleDefinition";
            Id.TypeId = "UpgradeModule";
        }
    }
}