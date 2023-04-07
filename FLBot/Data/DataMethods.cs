using FLBot.Models;
using FreeLanceBot.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace FLBot.Data
{
    /// <summary>
    /// Для работы с бд
    /// </summary>
    public static class DataMethods
    {
        /// <summary>
        /// Актуальные фильтры
        /// </summary>
        public static event Func<int, ITelegramBotClient, List<OrderFilter>, Task> ActualFilters; //Событие при создании нового заказа, для отправки его работникам
        /// <summary>
        /// Актуальные заказы
        /// </summary>
        public static event Func<long, ITelegramBotClient, List<Order>, Task> ActualOrders; //Событие для отправки работнику актуальных заказов, когда создан фильтр
        /// <summary>
        /// Добавление клиента
        /// </summary>
        public static async Task AddClient(long idTelegram, string firstName, string? lastName, string? userName, long idChat)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                if(lastName == null) { lastName = "пусто"; }
                if(userName == null) { userName = "пусто"; }
                Client client = new Client()
                {
                    IdTelegram = idTelegram,
                    IdChat = idChat,
                    FirstName = firstName,
                    LastName = lastName,
                    UserName = userName,
                    PhoneNumber = "пусто",
                    Working = false,
                    CountResponse = 0,
                    DateResponse = default                    
                };

                var clientCheck = await botContext.Clients.FirstOrDefaultAsync(a=> a.IdTelegram == idTelegram);
                if (clientCheck != null) { return; }    
                
                await botContext.Clients.AddAsync(client);
                await botContext.SaveChangesAsync();
            }
        }
        #region Order
        /// <summary>
        /// Добавление заказа с начальными параметрами
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <returns></returns>
        public static async Task<int> AddNewOrder(long idTelegram, int idCity)
        {
            using(FLBotContext botContext = new FLBotContext()) 
            {
                var city = await botContext.Cities.FirstOrDefaultAsync(a => a.Id == idCity);
                Order order = new Order()
                {
                    IdCustomer = idTelegram,
                    IdCity = city.Id,
                    Status = "Неактуален",
                    DateCreate = DateTime.Now,
                    Favorites = false
                };
                await botContext.Orders.AddAsync(order);
                await botContext.SaveChangesAsync();
                int idOrder = order.Id;
                return idOrder;
            }
        }
        /// <summary>
        /// Добавление категории в заказ
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="idOrder"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static async Task<bool> AddCategoryOrder(long idTelegram, int idOrder, int idCategory)
        {
            using (FLBotContext botContext = new FLBotContext())
            {               
                var category = await botContext.Categories.FirstOrDefaultAsync(a => a.Id == idCategory);
                var order = await botContext.Orders.FirstOrDefaultAsync(a=>a.Id == idOrder);
                order.IdCategory = category.Id;
                await botContext.SaveChangesAsync();

                var subCategories = await botContext.SubCategories.Where(a => a.IdCategory == category.Id).ToListAsync();
                if (subCategories.Count != 0) { return true; }
                else
                {
                    order.IdSubCategory = 1;
                    await botContext.SaveChangesAsync();
                    return false;
                }              
            }           
        }
        /// <summary>
        /// Добавление подкатегории
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="idOrder"></param>
        /// <param name="subCategory"></param>
        /// <returns></returns>
        public static async Task<int> AddSubCategoryOrder(long idTelegram, int idOrder, int idSubCategory)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var subCategory = await botContext.SubCategories.FirstOrDefaultAsync(a => a.Id == idSubCategory);
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                order.IdSubCategory = subCategory.Id;
                await botContext.SaveChangesAsync();
                return idOrder;
            }
        }
        /// <summary>
        /// Добавление района
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="idOrder"></param>
        /// <param name="district"></param>
        /// <returns></returns>
        public static async Task<int> AddDistrictOrder(long idTelegram, int idOrder, int idDistrict)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var district = await botContext.Districts.FirstOrDefaultAsync(a => a.Id == idDistrict);
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                order.IdDistrict = district.Id;
                await botContext.SaveChangesAsync();
                return idOrder;
            }
        }
        /// <summary>
        /// Добавление адреса
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="idOrder"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static async Task<int> AddAddressOrder(long idTelegram, int idOrder, string address)
        {
            using(FLBotContext botContext = new FLBotContext())
            {
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                order.Address = address;
                await botContext.SaveChangesAsync();
                return idOrder;
            }
        }
        /// <summary>
        /// Добаление графика работы
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="idOrder"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static async Task<int> AddScheduleOrder(long idTelegram, int idOrder, string schedule)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                order.Schedule = schedule;
                await botContext.SaveChangesAsync();
                return idOrder;
            }
        }
        /// <summary>
        /// Добавление размера оплаты
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="idOrder"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        public static async Task<int> AddPaymentOrder(long idTelegram, int idOrder, string payment)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                order.Payment = payment;
                await botContext.SaveChangesAsync();
                return idOrder;
            }
        }
        /// <summary>
        /// Добавление описания
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="idOrder"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static async Task<int> AddDescriptionOrder(long idTelegram, int idOrder, string description)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                order.Description = description;
                await botContext.SaveChangesAsync();
                return idOrder;
            }
        }
        /// <summary>
        /// Публикация
        /// </summary>
        /// <param name="idOrder"></param>
        /// <returns></returns>
        public static async Task<int> PublishOrder(int idOrder, ITelegramBotClient botClient)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                order.Status = "Создан";
                await botContext.SaveChangesAsync();
                await ActualFilters.Invoke(idOrder, botClient, await GetActualFilters(idOrder));
                return order.Id;
            }
        }
        /// <summary>
        /// Добавление заказа в избранное
        /// </summary>
        /// <param name="idEntity"></param>
        /// <returns></returns>
        public static async Task AddFavotitesOrder(int idOrder)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                order.Favorites = true;
                await botContext.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Возвращает избранные заказы
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <returns></returns>
        public static async Task<List<Order>> GetFavoriteOrders(long idTelegram)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                return await botContext.Orders.Where(a => a.IdCustomer == idTelegram &&
                                                           a.Favorites == true).ToListAsync();
            }

        }
        #endregion
        /// <summary>
        /// Поиск актуальных заказов у работадателя
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <returns></returns>
        public static async Task<List<Order>> GetOrders(long idTelegram)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                return await botContext.Orders.Where(a => a.IdCustomer == idTelegram &&
                                                          a.Status == "Создан").ToListAsync();
            }
        }
        /// <summary>
        /// Поиск актуальных фильтов у сотрудника
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <returns></returns>
        public static async Task<List<OrderFilter>> GetFilters(long idTelegram)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                return await botContext.Filters.Where(a => a.IdTelegram == idTelegram &&
                                                           a.Status == "Актуален").ToListAsync();
            }
        }
        /// <summary>
        /// Поиск актуальных работников подходящих заказу
        /// </summary>
        /// <param name="idOrder"></param>
        /// <returns></returns>
        public static async Task<List<OrderFilter>> GetActualFilters(int idOrder)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                var filters = new List<OrderFilter>();
                filters = await botContext.Filters.Where(a => a.Status == "Актуален" &&
                                                              a.IdCity == order.IdCity &&
                                                              (a.IdDistrict == order.IdDistrict || a.IdDistrict == 0) &&
                                                              a.IdCategory == order.IdCategory &&
                                                              a.IdSubCategory == order.IdSubCategory).ToListAsync();
                return filters;
            }
        }
        /// <summary>
        /// Добаление телефона
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static async Task AddPhone(long idTelegram, string phone)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var client = await botContext.Clients.FirstOrDefaultAsync(a => a.IdTelegram == idTelegram);
                client.PhoneNumber = phone;
                await botContext.SaveChangesAsync();               
            }
        }       

        #region Filter

        /// <summary>
        /// Добаление нового фильтра
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public static async Task<int> AddNewFilter(long idTelegram, int idCity, long idChat)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var city = await botContext.Cities.FirstOrDefaultAsync(a => a.Id == idCity);
                OrderFilter filter = new OrderFilter()
                {
                    IdTelegram = idTelegram,
                    IdCity = city.Id,
                    Status = "Неактуален",
                    IdChat = idChat,
                    Favorites = false
                };
                await botContext.Filters.AddAsync(filter);
                await botContext.SaveChangesAsync();
                int idFilter = filter.Id;
                return idFilter;
            }          
        }
        /// <summary>
        /// Добавление категории
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="idFilter"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static async Task<bool> AddCategoryFilter(long idTelegram, int idFilter, int idCategory)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var category = await botContext.Categories.FirstOrDefaultAsync(a => a.Id == idCategory);
                var filter = await botContext.Filters.FirstOrDefaultAsync(a => a.Id == idFilter);
                filter.IdCategory = category.Id;
                await botContext.SaveChangesAsync();

                var subCategories = await botContext.SubCategories.Where(a => a.IdCategory == category.Id).ToListAsync();
                if (subCategories.Count != 0) { return true; }
                else
                {
                    filter.IdSubCategory = 1;
                    await botContext.SaveChangesAsync();
                    return false;
                }
                
            }
        }
        /// <summary>
        /// Добавление подкатегории
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="idFilter"></param>
        /// <param name="subCategory"></param>
        /// <returns></returns>
        public static async Task<int> AddSubCategoryFilter(long idTelegram, int idFilter, int idSubCategory)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var subCategory = await botContext.SubCategories.FirstOrDefaultAsync(a => a.Id == idSubCategory);
                var filter = await botContext.Filters.FirstOrDefaultAsync(a => a.Id == idFilter);
                filter.IdSubCategory = subCategory.Id;
                await botContext.SaveChangesAsync();
                return idFilter;
            }
        }
        /// <summary>
        /// Добавление района
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="idFilter"></param>
        /// <param name="district"></param>
        /// <returns></returns>
        public static async Task<int> AddDistrictFilter(long idTelegram, int idFilter, int idDistrict)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var filter = await botContext.Filters.FirstOrDefaultAsync(a => a.Id == idFilter);
                if (idDistrict == 0)//Если ноль, значит район непринципиален
                {
                    filter.IdDistrict = 0;
                }
                else
                {
                    var district = await botContext.Districts.FirstOrDefaultAsync(a => a.Id == idDistrict);                   
                    filter.IdDistrict = district.Id;
                }
                await botContext.SaveChangesAsync();
                return idFilter;
            }
        }
        /// <summary>
        /// Добавление, изменение имени для фильтра
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task AddNameFilter(long idTelegram, string name, int idFilter)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var client = await botContext.Clients.FirstOrDefaultAsync(a => a.IdTelegram == idTelegram);
                var filter = await botContext.Filters.FirstOrDefaultAsync(a => a.Id == idFilter);
                filter.Name = name;
                client.FirstName = name;
                await botContext.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Добавление телефона в фильтр
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="phone"></param>
        /// <param name="idFilter"></param>
        /// <returns></returns>
        public static async Task AddPhoneFilter(long idTelegram, string phone, int idFilter)
        {
            await AddPhone(idTelegram, phone);
            using(FLBotContext botContext = new FLBotContext())
            {
                var filter = await botContext.Filters.FirstOrDefaultAsync(a => a.Id == idFilter);
                filter.TelephoneNumber = phone;
                await botContext.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Публикация фильтра
        /// </summary>
        /// <param name="idFilter"></param>
        /// <returns></returns>
        public static async Task<bool> PublishFilter( long idTelegram,long idChat,int idFilter, ITelegramBotClient botClient)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var filter = await botContext.Filters.FirstOrDefaultAsync(a => a.Id == idFilter);
                filter.Status = "Актуален";
                await botContext.SaveChangesAsync();
                await ActualOrders.Invoke(idChat, botClient,await GetActualOrders(filter.Id));
                return true;
            }
        }
        /// <summary>
        /// Поиск актуальных заказов подходящих к фильтру
        /// </summary>
        /// <param name="idFilter"></param>
        /// <returns></returns>
        public static async Task<List<Order>> GetActualOrders(int idFilter)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var filter = await botContext.Filters.FirstOrDefaultAsync(a => a.Id == idFilter);
                var orders = new List<Order>();
                if(filter.IdDistrict == 0)
                {
                    orders = await botContext.Orders.Where(e => e.Status == "Создан" &&
                                                                e.IdCity == filter.IdCity &&
                                                                e.IdCategory == filter.IdCategory &&
                                                                e.IdSubCategory == filter.IdSubCategory).ToListAsync();
                }
                else
                {
                    orders = await botContext.Orders.Where(e => e.Status == "Создан" &&
                                                                e.IdCity == filter.IdCity &&
                                                                e.IdDistrict == filter.IdDistrict &&
                                                                e.IdCategory == filter.IdCategory &&
                                                                e.IdSubCategory == filter.IdSubCategory).ToListAsync();
                }
                return orders;
            }            
        }
        /// <summary>
        /// перевод статуса заказа в в архив
        /// </summary>
        /// <param name="idOrder"></param>
        /// <returns></returns>
        public static async Task ArchiveOrder(int idOrder)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                order.Status = "Архив";
                await botContext.SaveChangesAsync();

            }
        }
        /// <summary>
        /// переводит статус фильтра в архив
        /// </summary>
        /// <param name="idFilter"></param>
        /// <returns></returns>
        public static async Task ArchiveFilter(int idFilter)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var filter = await botContext.Filters.FirstOrDefaultAsync(a => a.Id == idFilter);
                filter.Status = "Архив";
                await botContext.SaveChangesAsync();

            }
        }
        /// <summary>
        /// Добавление фильтра в избранное
        /// </summary>
        /// <param name="idEntity"></param>
        /// <returns></returns>
        public static async Task AddFavotitesFilter(int idFilter)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var filter = await botContext.Filters.FirstOrDefaultAsync(a => a.Id == idFilter);
                filter.Favorites = true;
                await botContext.SaveChangesAsync();
            }
        }
        /// <summary>
        /// возвращает избранные фильтры
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <returns></returns>
        public static async Task<List<OrderFilter>> GetFavoriteFilters(long idTelegram)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                return await botContext.Filters.Where(a => a.IdTelegram == idTelegram &&
                                                           a.Favorites == true).ToListAsync();
            }
               
        }
        #endregion
    }
}
