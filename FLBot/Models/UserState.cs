using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLBot.Models
{
    public class UserState
    {
        /// <summary>
        /// id заказа
        /// </summary>
        public int IdOrder { get; set; }
        /// <summary>
        /// id фильтра для поиска
        /// </summary>
        public int IdFilter { get; set; }
        /// <summary>
        /// Состояние
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Роль(Работодатель, Работник)
        /// </summary>
        public string Role { get; set; }
    }
}
