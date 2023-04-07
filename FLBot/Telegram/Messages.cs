using FLBot.Data;
using FLBot.Models;
using FreeLanceBot.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace FLBot.Telegram
{
    public static class Messages
    {
        /// <summary>
        /// Приветственное сообщение
        /// </summary>
        /// <param name="firstName"></param>
        /// <returns></returns>
        public static string HelloMessage(string firstName)
        {
            return $"Привет {firstName} с помощью этого бота ты можешь найти сотрудника/работу в 1 клик";
        }
        /// <summary>
        /// Информация о заказе
        /// </summary>
        /// <param name="idOrder"></param>
        /// <returns></returns>
        public static async Task<string> OrderMessage(int idOrder)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                var city = await botContext.Cities.FirstOrDefaultAsync(a => a.Id == order.IdCity);
                var district = await botContext.Districts.FirstOrDefaultAsync(a => a.Id == order.IdDistrict);
                var address = order.Address;
                var category = await botContext.Categories.FirstOrDefaultAsync(a => a.Id == order.IdCategory);
                var subCategory = await botContext.SubCategories.FirstOrDefaultAsync(a => a.Id==order.IdSubCategory);
                var payment = order.Payment;
                var schedule = order.Schedule;
                var description = order.Description;
                return $"Город: {city.Name}\n" +
                    $"Район: {district.Name}\n" +
                    $"Адрес: {address}\n" +
                    $"Категория: {category.Name}\n" +
                    $"Подкатегория: {subCategory.Name}\n" +
                    $"Оплата: {payment}\n" +
                    $"График работы: {schedule}\n" +
                    $"Описание: {description}";
            }
        }
        /// <summary>
        /// Информация о фильтре
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="idFilter"></param>
        /// <returns></returns>
        public static async Task<string> FilterMessage(int idFilter)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var filter = await botContext.Filters.FirstOrDefaultAsync(a => a.Id == idFilter);
                var city = await botContext.Cities.FirstOrDefaultAsync(a => a.Id == filter.IdCity);
                var district = new District();            
                if (filter.IdDistrict == 0) { district.Name = "Любой"; }
                else { district = await botContext.Districts.FirstOrDefaultAsync(a => a.Id == filter.IdDistrict); }
                var category = await botContext.Categories.FirstOrDefaultAsync(a => a.Id == filter.IdCategory);
                var subCategory = await botContext.SubCategories.FirstOrDefaultAsync(a => a.Id == filter.IdSubCategory);
                return $"Имя: {filter.Name}\n" +
                    $"Телефон: {filter.TelephoneNumber}\n" +
                    $"Категория: {category.Name}\n" +
                    $"Подкатегория: {subCategory.Name}\n" +
                    $"Город: {city.Name}\n" +
                    $"Район: {district.Name}";
            }
        }
        /// <summary>
        /// Отправка сообщений с актуальными заказами одному работнику
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="idChat"></param>
        /// <returns></returns>
        public static async Task SendOrders(long idChat, ITelegramBotClient botClient, List<Order> orders)
        {
            if(orders.Count == 0) 
            { await botClient.SendTextMessageAsync(idChat,"Пока заказов с данными параметрами нет. Ожидайте"); return; }
            foreach (var order in orders)
            {             
                    await botClient.SendTextMessageAsync(
                                idChat,
                                text: "Список актуальных заявок");
                    await botClient.SendTextMessageAsync(
                                idChat,
                                text: await OrderMessage(order.Id),
                                replyMarkup: Keyboards.Response(order.Id));                
            }
        }
        /// <summary>
        /// Отправка сообщений клиенту с актуальными заказами нескольким работникам
        /// </summary>
        /// <param name="idOrder"></param>
        /// <param name="botClient"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static async Task SendOrders(int idOrder, ITelegramBotClient botClient, List<OrderFilter> filters)
        {
            using(FLBotContext botContext = new FLBotContext())
            {
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                string message = await OrderMessage(idOrder);
                foreach(var filter in filters) 
                {
                    await botClient.SendTextMessageAsync(
                        filter.IdChat,
                        text: "Новая заявка для вас."
                        );
                    await botClient.SendTextMessageAsync(
                        filter.IdChat,
                        text: message,
                        replyMarkup: Keyboards.Response(idOrder)
                        );
                }
            }
        }
        /// <summary>
        /// Сообщение работодателю при отклике на заказ
        /// </summary>
        /// <param name="idOrder"></param>
        /// <param name="idTelegram"></param>
        /// <param name="botClient"></param>
        /// <param name="idChat"></param>
        /// <returns></returns>
        public static async Task ResponseMessage(int idOrder, long idTelegram, ITelegramBotClient botClient)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var order = await botContext.Orders.FirstOrDefaultAsync(a => a.Id == idOrder);
                var employee = await botContext.Clients.FirstOrDefaultAsync(a => a.IdTelegram == idTelegram);
                var employer = await botContext.Clients.FirstOrDefaultAsync(a => a.IdTelegram == order.IdCustomer);
                string message;               
                bool check = Payment.Check(employer.Id);
                if (check)
                {
                    message = $"На вашу заявку с Id: {order.Id} пришел отклик.\n" +
                                $"Имя: {employee.FirstName}\n" +
                                $"Номер: {employee.PhoneNumber}";
                }
                else
                {
                    message = $"На вашу заявку с Id: {order.Id} пришел отклик.\n" +
                                $"Имя: {employee.FirstName}\n" +
                                $"Номер: \"скрыт\".\n" +
                                $"Нужно оплатить подписку";
                }
                await botClient.SendTextMessageAsync(
                        employer.IdChat,
                        text: message,
                        replyMarkup: Keyboards.Agreed(idOrder)
                        );
            }
        }
        /// <summary>
        /// отправка актуальных заказов клиенту
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="botClient"></param>
        /// <param name="idChat"></param>
        /// <returns></returns>
        public static async Task ActualOrders(long idTelegram, ITelegramBotClient botClient, long idChat)
        {
            List<Order> orders = await DataMethods.GetOrders(idTelegram);
            if(orders.Count == 0) 
            {
                await botClient.SendTextMessageAsync(
                    idChat,
                    text: "У вас нет актуальных заказов"
                    );
                return;
            }
            foreach (Order order in orders)
            {
                await botClient.SendTextMessageAsync(
                    idChat,
                    text: await OrderMessage(order.Id),
                    replyMarkup: Keyboards.ArchiveOrder(order.Id)
                    );
            }
        }
        /// <summary>
        /// отправка актуальных фильтров клиенту
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="botClient"></param>
        /// <param name="idChat"></param>
        /// <returns></returns>
        public static async Task ActualFilters(long idTelegram, ITelegramBotClient botClient, long idChat)
        {
            List<OrderFilter> filters = await DataMethods.GetFilters(idTelegram);
            if (filters.Count == 0)
            {
                await botClient.SendTextMessageAsync(
                    idChat,
                    text: "У вас нет актуальных фильтров"
                    );
                return;
            }
            foreach (OrderFilter filter in filters)
            {
                await botClient.SendTextMessageAsync(
                    idChat,
                    text: await FilterMessage(filter.Id),
                    replyMarkup: Keyboards.ArchiveFilter(filter.Id));
            }
        }
        /// <summary>
        /// Добавление района в зависимости от города
        /// </summary>
        /// <param name="idEntity"></param>
        /// <param name="botClient"></param>
        /// <param name="idChat"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task AddDistrict(int idEntity, ITelegramBotClient botClient, long idChat, string role)
        {
            using(FLBotContext botContext = new FLBotContext())
            {
                int? IdCity;
                if (role == "Работодатель") 
                { var order = botContext.Orders.FirstOrDefault(a => a.Id == idEntity); IdCity = order.IdCity; }
                else 
                { var filter = botContext.Filters.FirstOrDefault(a => a.Id == idEntity); IdCity = filter.IdCity; }

                switch(IdCity)
                {
                    case 1:
                        using (var photoStream = new FileStream("./PhotoDistricts/TumenDistricts.png", FileMode.Open))
                        {
                            var inputPhoto = new InputOnlineFile(photoStream);
                            await botClient.SendPhotoAsync(
                                idChat,
                                photo: inputPhoto,
                                caption: "Выберите район",
                                replyMarkup: await Keyboards.District(idEntity, role, IdCity)
                                );
                        }
                        break;
                }                
            }          
        }
        /// <summary>
        /// Отправка избранных заказов
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="botClient"></param>
        /// <param name="idChat"></param>
        /// <returns></returns>
        public static async Task SendFavoriteOrders(long idTelegram, ITelegramBotClient botClient, long idChat)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var orders = await DataMethods.GetFavoriteOrders(idTelegram);
                if(orders.Count != 0)
                {
                    await botClient.SendTextMessageAsync(
                            idChat,
                            text: "Ваши избранные заказы");
                    foreach (var order in orders)
                    {
                        await botClient.SendTextMessageAsync(
                            idChat,
                            text: await OrderMessage(order.Id),
                            replyMarkup: Keyboards.GetOrder(order.Id));
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                            idChat,
                            text: "К сожалению у вас нет избранных заказов");
                }
            }
        }
        /// <summary>
        /// Отправка избранных фильтров
        /// </summary>
        /// <param name="idTelegram"></param>
        /// <param name="botClient"></param>
        /// <param name="idChat"></param>
        /// <returns></returns>
        public static async Task SendFavoriteFilters(long idTelegram, ITelegramBotClient botClient, long idChat)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var orders = await DataMethods.GetFavoriteFilters(idTelegram);
                if (orders.Count != 0)
                {
                    await botClient.SendTextMessageAsync(
                            idChat,
                            text: "Ваши избранные фильтры");
                    foreach (var order in orders)
                    {
                        await botClient.SendTextMessageAsync(
                            idChat,
                            text: await FilterMessage(order.Id),
                            replyMarkup: Keyboards.GetFilter(order.Id));
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                            idChat,
                            text: "К сожалению у вас нет избранных фильтров");
                }
            }
        }

    }
}
