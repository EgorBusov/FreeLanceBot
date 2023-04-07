using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeLanceBot.Models
{
    /// <summary>
    /// Клиент
    /// </summary>
    public class Client
    {
        public Client() { }
        public Client(long idTelegram, string firstName, string lastName, string userName, 
            string phoneNumber, bool working, int countResponse, DateTime dateResponse)
        {
            IdTelegram = idTelegram;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            PhoneNumber = phoneNumber;
            Working = working;
            CountResponse = countResponse;
            DateResponse = dateResponse;
        }

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id телеги
        /// </summary>
        public long IdTelegram { get; set; }
        /// <summary>
        /// Id чата с клиентом
        /// </summary>
        public long IdChat { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string? LastName { get; set; }
        /// <summary>
        /// Ник тг
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// Номер телефона
        /// </summary>
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// Выполняет заказ или нет
        /// </summary>
        public bool Working { get; set; }
        /// <summary>
        /// Сколько раз может получить номер исполнителя
        /// </summary>
        public int CountResponse { get; set; }
        /// <summary>
        /// До какого времени может получать номер исполнителя
        /// </summary>
        public DateTime DateResponse { get; set; }
    }
}
