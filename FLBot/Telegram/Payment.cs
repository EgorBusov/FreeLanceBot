using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLBot.Telegram
{
    /// <summary>
    /// Класс для олаты
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// Определяет наличие возможности принять отклик
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <returns></returns>
        public static bool Check(long idTelegram)
        {
            return true;
        }
    }
}
