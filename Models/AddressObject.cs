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
        public string ObjectId { get; set; }

        /// Глобальный уникальный идентификатор
        public string ObjectGuid { get; set; }

        /// Наименование адресного объекта
        public string Name { get; set; }

        /// Краткое наименование типа объекта (г., ул., пр. и т.д.)
        public string TypeName { get; set; }

        /// Уровень адресного объекта в иерархии
        public int Level { get; set; }

        /// Дата последнего обновления
        public DateTime UpdateDate { get; set; }
    }
}
