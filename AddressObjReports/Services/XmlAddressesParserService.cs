using System.Xml;
using AddressObjReports.Models;

namespace AddressObjReports.Services
{
    /// <summary>
    /// Класс, который представляет сервис работы с адресами.
    /// </summary>
    public class XmlAddressesParserService
    {
        private const string ObjectElementName = "OBJECT";
        private const string ObjectLevelElementName = "OBJECTLEVEL";
        private const string IsActiveAttribute = "ISACTIVE";
        private const string NameAttribute = "NAME";
        private const string LevelAttribute = "LEVEL";
        private const string TypeNameAttribute = "TYPENAME";
        
        //Названия Object Level по которым не нужно указывать информацию.
        private readonly HashSet<string> _unusedLevels =
        [
            "Здание (строение), сооружение",
            "Помещение",
            "Помещения в пределах помещения",
            "Земельный участок",
            "Машино-место"
        ];

        /// <summary>
        /// Возвращает сгруппированные адреса в виде словаря с ключом LevelName и значением списка адресов.
        /// </summary>
        /// <param name="directoryName">Имя каталога, куда был распакован zip файл.</param>
        /// <returns></returns>
        public Dictionary<string, List<Address>> GetGroupAddresses(string directoryName)
        {
            var addresses = new List<Address>();
            var directories = Directory.GetDirectories(directoryName);
            var objectLevels = new Dictionary<int, string>();

            GetLevelNames(objectLevels, directoryName);
            
            foreach (var directory in directories)
            {
                var files = Directory.GetFiles(directory, $"AS_ADDR_OBJ_{DateTime.Now.Year}*");
                if (files.Length == 0) continue;
                
                AddAddresses(addresses, files[0], objectLevels);
            }

            var groupedAddresses = addresses
                .OrderBy(address => address.Name)
                .GroupBy(a => a.LevelName)
                .ToDictionary(x => x.Key, y => y.ToList());

            return groupedAddresses;
        }

        private void GetLevelNames(Dictionary<int, string> objectLevels, string directoryName)
        {
            var files = Directory.GetFiles(directoryName, "AS_OBJECT_LEVELS*");
            if (files.Length == 0) return;

            using var xmlReader = XmlReader.Create(files[0]);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == ObjectLevelElementName)
                {
                    if (xmlReader.HasAttributes)
                    {
                        var name = xmlReader.GetAttribute("NAME");
                        var level = xmlReader.GetAttribute("LEVEL");
                
                        if (!string.IsNullOrEmpty(name) && 
                            !string.IsNullOrEmpty(level) &&
                            !_unusedLevels.Contains(name))
                        {
                            objectLevels[int.Parse(level)] = name;
                        }
                    }
                }
            }
        }

        private void AddAddresses(List<Address> addresses, string path, Dictionary<int, string> objectLevels)
        {
            using var xmlReader = XmlReader.Create(path);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == ObjectElementName)
                {
                    if (!xmlReader.HasAttributes) continue;
                    
                    var isActive = xmlReader.GetAttribute(IsActiveAttribute);
                    if (isActive != "1") continue;
                    
                    var levelStr = xmlReader.GetAttribute(LevelAttribute);
                    if (string.IsNullOrEmpty(levelStr)) continue;
                    
                    if (!int.TryParse(levelStr, out var level)) continue;
                    
                    if (!objectLevels.ContainsKey(level)) continue;
                    
                    var typeName = xmlReader.GetAttribute(TypeNameAttribute);
                    var name = xmlReader.GetAttribute(NameAttribute);
                    
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(typeName)) continue;
                    
                    var address = new Address
                    {
                        LevelName = objectLevels[level],
                        TypeName = typeName,
                        Name = name,
                        Level = level
                    };
                    
                    addresses.Add(address);
                }
            }
        }
    }
}
