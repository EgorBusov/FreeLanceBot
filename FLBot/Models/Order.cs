using FreeLanceBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeLanceBot.Models
{
    /// <summary>
    /// Заказ
    /// </summary>
    public class Order
    {
        public Order() { }
        public Order(int Id,long IdCustomer, int IdCity, int IdDistrict, string Address, int IdCategory, int IdSubCategory,
                            string Payment, string Description, string Status/*, long IdExecutor*/)
        {
            this.Id = Id;
            this.IdCustomer = IdCustomer;
            this.IdCity = IdCity;
            this.IdDistrict = IdDistrict;
            this.Address = Address;
            this.IdCategory = IdCategory;
            this.IdSubCategory = IdSubCategory;
            this.Payment = Payment;
            this.Description = Description;
            this.Status = Status;
            //this.IdExecutor = IdExecutor;
        }
        public int Id { get; set; }
        /// <summary>
        /// Id заказчика совпадает с id в тг
        /// </summary>
        public long IdCustomer { get; set; }
        /// <summary>
        /// Id города
        /// </summary>
        public int? IdCity { get; set; }
        /// <summary>
        /// Id района
        /// </summary>
        public int? IdDistrict { get; set; }
        /// <summary>
        /// Адрес исполнения
        /// </summary>
        public string? Address { get; set; }
        /// <summary>
        /// Id категории
        /// </summary>
        public int? IdCategory { get; set; }
        /// <summary>
        /// Id подкатегории
        /// </summary>
        public int? IdSubCategory { get; set; }
        /// <summary>
        /// Стоимость услуг
        /// </summary>
        public string? Payment { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Статус(Неактуален, Создан, Архив)
        /// </summary>
        public string Status { get; set; }
        ///// <summary>
        ///// Id исполнителя
        ///// </summary>
        //public long? IdExecutor { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime DateCreate { get; set; }
        /// <summary>
        /// График работы
        /// </summary>
        public string? Schedule { get; set; }
        /// <summary>
        /// Избранный
        /// </summary>
        public bool Favorites { get; set; }
    }
}
