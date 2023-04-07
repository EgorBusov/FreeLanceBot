using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLBot.Models
{
    /// <summary>
    /// Фильтр для поиска заказов
    /// </summary>
    public class OrderFilter
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id телеги
        /// </summary>
        public long IdTelegram { get; set; }
        /// <summary>
        /// Id чата 
        /// </summary>
        public long IdChat { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Id города
        /// </summary>
        public int? IdCity { get; set; }
        /// <summary>
        /// Id района(0 если работнику непринципиально)
        /// </summary>
        public int? IdDistrict { get; set; }
        /// <summary>
        /// Id категории
        /// </summary>
        public int IdCategory { get; set; }
        /// <summary>
        /// Id подкатегории
        /// </summary>
        public int IdSubCategory { get; set; }
        /// <summary>
        /// Номер телефона
        /// </summary>
        public string? TelephoneNumber { get; set; }
        /// <summary>
        /// Статус(Неактуален, Актуален, Архив)
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Избранный
        /// </summary>
        public bool Favorites { get; set; }

        



    }
}
