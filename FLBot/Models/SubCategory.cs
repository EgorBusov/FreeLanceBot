using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeLanceBot.Models
{
    /// <summary>
    /// Подкатегория
    /// </summary>
    public class SubCategory
    {
        /// <summary>
        /// Id Подкатегории
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id Категории
        /// </summary>
        public int IdCategory { get; set; }
    }
}
