using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenerator.Models
{
    internal class AddressObject
    {
        /// Идентификатор объекта
        public string ObjectId { get; set; } = string.Empty;

        /// Глобальный уникальный идентификатор
        public string ObjectGuid { get; set; } = string.Empty;

        /// Наименование адресного объекта
        public string Name { get; set; } = string.Empty;

        /// Краткое наименование типа объекта (г., ул., пр. и т.д.)
        public string TypeName { get; set; } = string.Empty;

        /// Уровень адресного объекта в иерархии
        public int Level { get; set; } = int.Parse("0");

        /// Дата последнего обновления
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }
}
