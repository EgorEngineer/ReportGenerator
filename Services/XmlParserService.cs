using ReportGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReportGenerator.Services
{
    internal class XmlParserService
    {
        /// Парсит адресные объекты из XML-файлов
        /// <param name="extractPath">Путь к папке с разархивированными файлами</param>
        /// <returns>Список адресных объектов</returns>
        public List<AddressObject> ParseAddressObjects(string extractPath)
        {
            var objects = new List<AddressObject>();

            var addrObjFiles = Directory.GetFiles(extractPath, "AS_ADDR_OBJ_*.XML", SearchOption.AllDirectories);

            foreach (var file in addrObjFiles)
            {
                var doc = XDocument.Load(file);
                var items = doc.Descendants("OBJECT")
                    .Where(x => x.Attribute("ISACTIVE")?.Value == "1")
                    .Select(x => new AddressObject
                    {
                        ObjectId = x.Attribute("OBJECTID")?.Value ?? string.Empty,
                        ObjectGuid = x.Attribute("OBJECTGUID")?.Value ?? string.Empty,
                        Name = x.Attribute("NAME")?.Value ?? string.Empty,
                        TypeName = x.Attribute("TYPENAME")?.Value ?? string.Empty,
                        Level = int.Parse(x.Attribute("LEVEL")?.Value ?? "0"),
                        UpdateDate = DateTime.Parse(x.Attribute("UPDATEDATE")?.Value ?? DateTime.Now.ToString())
                    });

                objects.AddRange(items);
            }

            return objects;
        }

        /// Парсит уровни объектов из XML-файлов
        /// <param name="extractPath">Путь к папке с разархивированными файлами</param>
        /// <returns>Словарь уровней (ключ - номер уровня, значение - название)</returns>
        public Dictionary<int, string> ParseObjectLevels(string extractPath)
        {
            var levels = new Dictionary<int, string>();

            var levelFiles = Directory.GetFiles(extractPath, "AS_OBJECT_LEVELS_*.XML", SearchOption.AllDirectories);

            if (levelFiles.Length > 0)
            {
                var doc = XDocument.Load(levelFiles[0]);
                foreach (var item in doc.Descendants("OBJECTLEVEL"))
                {
                    int level = int.Parse(item.Attribute("LEVEL")?.Value ?? "0");
                    string name = item.Attribute("NAME")?.Value ?? $"Уровень {level}";
                    levels[level] = name;
                }
            }

            return levels;
        }
    }
}
