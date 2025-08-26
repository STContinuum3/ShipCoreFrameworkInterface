using System.IO;
using System.Text;
using System.Xml;

namespace ShipClassInterface.Services
{
    public enum XmlFileType
    {
        Unknown,
        ShipCore,
        BlockGroups,
        WorldConfig,
        Manifest
    }

    public class XmlFileTypeDetector
    {
        public static XmlFileType DetectFileType(string filePath)
        {
            if (!File.Exists(filePath))
                return XmlFileType.Unknown;

            try
            {
                // Use the same encoding detection logic as the main XML loader
                using var streamReader = new StreamReader(filePath, detectEncodingFromByteOrderMarks: true);
                using var xmlReader = XmlReader.Create(streamReader);

                // Read to the first element
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        var rootElement = xmlReader.LocalName;
                        
                        return rootElement switch
                        {
                            "ShipCore" => XmlFileType.ShipCore,
                            "ArrayOfBlockGroup" => XmlFileType.BlockGroups,
                            "ModConfig" => XmlFileType.WorldConfig,
                            "ArrayOfString" => XmlFileType.Manifest,
                            "CoreManifest" => XmlFileType.Manifest,
                            _ => XmlFileType.Unknown
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Try fallback encoding detection for UTF-16 files
                try
                {
                    var fileBytes = File.ReadAllBytes(filePath);
                    if (fileBytes.Length >= 2)
                    {
                        StreamReader? streamReader = null;
                        
                        // Check for UTF-16 LE BOM (0xFF 0xFE)
                        if (fileBytes[0] == 0xFF && fileBytes[1] == 0xFE)
                        {
                            streamReader = new StreamReader(filePath, Encoding.Unicode);
                        }
                        // Check for UTF-16 BE BOM (0xFE 0xFF)
                        else if (fileBytes[0] == 0xFE && fileBytes[1] == 0xFF)
                        {
                            streamReader = new StreamReader(filePath, Encoding.BigEndianUnicode);
                        }

                        if (streamReader != null)
                        {
                            using (streamReader)
                            {
                                using var xmlReader = XmlReader.Create(streamReader);
                            
                                while (xmlReader.Read())
                                {
                                    if (xmlReader.NodeType == XmlNodeType.Element)
                                    {
                                        var rootElement = xmlReader.LocalName;
                                        
                                        return rootElement switch
                                        {
                                            "ShipCore" => XmlFileType.ShipCore,
                                            "ArrayOfBlockGroup" => XmlFileType.BlockGroups,
                                            "ModConfig" => XmlFileType.WorldConfig,
                                            "ArrayOfString" => XmlFileType.Manifest,
                                            "CoreManifest" => XmlFileType.Manifest,
                                            _ => XmlFileType.Unknown
                                        };
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // If all attempts fail, return unknown
                }
                
                return XmlFileType.Unknown;
            }

            return XmlFileType.Unknown;
        }

        public static string GetFileTypeDescription(XmlFileType fileType)
        {
            return fileType switch
            {
                XmlFileType.ShipCore => "Ship Core Configuration",
                XmlFileType.BlockGroups => "Block Groups Configuration", 
                XmlFileType.WorldConfig => "World Configuration",
                XmlFileType.Manifest => "Manifest Configuration",
                XmlFileType.Unknown => "Unknown XML File",
                _ => "Unknown"
            };
        }

        public static string GetRecommendedFileName(XmlFileType fileType)
        {
            return fileType switch
            {
                XmlFileType.ShipCore => "ShipCoreConfig_*.xml",
                XmlFileType.BlockGroups => "ShipCoreConfig_Groups.xml",
                XmlFileType.WorldConfig => "ShipCoreConfig_World.xml",
                XmlFileType.Manifest => "CoreManifest.xml",
                _ => "*.xml"
            };
        }
    }
}