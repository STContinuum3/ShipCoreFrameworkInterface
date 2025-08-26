using ShipClassInterface.Models;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ShipClassInterface.Services
{
    public class XmlService
    {
        private XmlSerializerNamespaces GetNamespaces()
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            return namespaces;
        }

        private XmlWriterSettings GetWriterSettings()
        {
            return new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                Encoding = Encoding.UTF8 // UTF-8
            };
        }

        public ShipCore? LoadShipCore(string filePath)
        {
            return LoadXml<ShipCore>(filePath);
        }

        private T? LoadXml<T>(string filePath) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            
            try
            {
                // Use StreamReader with automatic encoding detection (handles BOM)
                using var streamReader = new StreamReader(filePath, detectEncodingFromByteOrderMarks: true);
                using var xmlReader = XmlReader.Create(streamReader);
                return serializer.Deserialize(xmlReader) as T;
            }
            catch (Exception)
            {
                // Fallback: Try reading as plain text first to see encoding
                var fileBytes = File.ReadAllBytes(filePath);
                if (fileBytes.Length >= 2)
                {
                    // Check for UTF-16 LE BOM (0xFF 0xFE)
                    if (fileBytes[0] == 0xFF && fileBytes[1] == 0xFE)
                    {
                        using var streamReader = new StreamReader(filePath, Encoding.Unicode);
                        using var xmlReader = XmlReader.Create(streamReader);
                        return serializer.Deserialize(xmlReader) as T;
                    }
                    // Check for UTF-16 BE BOM (0xFE 0xFF)
                    else if (fileBytes[0] == 0xFE && fileBytes[1] == 0xFF)
                    {
                        using var streamReader = new StreamReader(filePath, Encoding.BigEndianUnicode);
                        using var xmlReader = XmlReader.Create(streamReader);
                        return serializer.Deserialize(xmlReader) as T;
                    }
                }
                
                // Re-throw original exception if all attempts fail
                throw;
            }
        }

        public void SaveShipCore(ShipCore shipCore, string filePath)
        {
            var serializer = new XmlSerializer(typeof(ShipCore));
            var settings = GetWriterSettings();
            var namespaces = GetNamespaces();

            using var writer = XmlWriter.Create(filePath, settings);
            serializer.Serialize(writer, shipCore, namespaces);
        }

        public BlockGroupCollection? LoadBlockGroups(string filePath)
        {
            return LoadXml<BlockGroupCollection>(filePath);
        }

        public void SaveBlockGroups(BlockGroupCollection groups, string filePath)
        {
            var serializer = new XmlSerializer(typeof(BlockGroupCollection));
            var settings = GetWriterSettings();
            var namespaces = GetNamespaces();

            using var writer = XmlWriter.Create(filePath, settings);
            serializer.Serialize(writer, groups, namespaces);
        }

        public WorldConfig? LoadWorldConfig(string filePath)
        {
            return LoadXml<WorldConfig>(filePath);
        }

        public void SaveWorldConfig(WorldConfig config, string filePath)
        {
            var serializer = new XmlSerializer(typeof(WorldConfig));
            var settings = GetWriterSettings();
            var namespaces = GetNamespaces();

            using var writer = XmlWriter.Create(filePath, settings);
            serializer.Serialize(writer, config, namespaces);
        }

        public Manifest? LoadManifest(string filePath)
        {
            try
            {
                // Try to load as new CoreManifest format
                return LoadXml<Manifest>(filePath);
            }
            catch
            {
                // If that fails, try to load as old ArrayOfString format
                try
                {
                    var serializer = new XmlSerializer(typeof(string[]), new XmlRootAttribute("ArrayOfString"));
                    
                    using var streamReader = new StreamReader(filePath, detectEncodingFromByteOrderMarks: true);
                    using var xmlReader = XmlReader.Create(streamReader);
                    var stringArray = serializer.Deserialize(xmlReader) as string[];
                    
                    if (stringArray != null)
                    {
                        var manifest = new Manifest();
                        foreach (var item in stringArray)
                        {
                            manifest.ShipCoreFiles.Add(item);
                        }
                        return manifest;
                    }
                }
                catch
                {
                    // If both fail, return null
                }
                
                return null;
            }
        }

        public void SaveManifest(Manifest manifest, string filePath)
        {
            var serializer = new XmlSerializer(typeof(Manifest));
            var settings = GetWriterSettings();
            var namespaces = GetNamespaces();

            using var writer = XmlWriter.Create(filePath, settings);
            serializer.Serialize(writer, manifest, namespaces);
        }
    }
}