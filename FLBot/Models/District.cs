using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeLanceBot.Models
{
    /// <summary>
    /// Район города
    /// </summary>
    public class District
    {
        /// <summary>
        /// Id района
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название района
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id города
        /// </summary>
        public int IdCity { get; set; }
    }
}
