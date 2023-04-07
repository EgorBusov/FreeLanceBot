using FLBot.Data;
using FreeLanceBot.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace FLBot.Telegram
{
    public static class Keyboards
    {
        /// <summary>
        /// Роли
        /// </summary>
        /// <returns></returns>
        public static InlineKeyboardMarkup Role()
        {
            return new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Ищу сотрудника", callbackData: "employer"),
                    InlineKeyboardButton.WithCallbackData(text: "Ищу работу", callbackData: "employee"),
                },
            };
        }    
        /// <summary>
        /// Города
        /// </summary>
        /// <returns></returns>
        public static async  Task<InlineKeyboardMarkup> Cities() 
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var cities = await botContext.Cities.ToListAsync();


                List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

                foreach(var c in cities) 
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData(text: c.Name, callbackData: $"CreateCity {c.Id}"));
                }
                InlineKeyboardMarkup inlines = new InlineKeyboardMarkup(buttons);
                return inlines;
            }
        }
        /// <summary>
        /// Категории
        /// </summary>
        /// <returns></returns>
        public static async Task<InlineKeyboardMarkup> Categories(int idEntity) 
        {
            using(FLBotContext botContext = new FLBotContext())
            {
                var categories = await botContext.Categories.ToListAsync();

                List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

                int i = 0;
                foreach (var c in categories)
                {
                    if (i % 3 == 0) { buttons.Add(new List<InlineKeyboardButton>()); }
                    buttons[buttons.Count - 1].Add
                        (InlineKeyboardButton.WithCallbackData(text: c.Name, callbackData: $"CreateCategory {c.Id} {idEntity}"));
                    i++;
                }
                
                InlineKeyboardMarkup inlines = new(buttons);
                return inlines;
            }
        }

        public static async Task<InlineKeyboardMarkup> SubCategories(int idEntity, int idCategory) 
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var category = await botContext.Categories.FirstOrDefaultAsync(a => a.Id == idCategory); //получение экземпляра категории, для поиска подкатегории
                var subCategories = await botContext.SubCategories.Where(a => a.IdCategory == category.Id).ToListAsync();

                List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

                int i = 0;
                foreach (var c in subCategories)
                {
                    if (i % 2 == 0) { buttons.Add(new List<InlineKeyboardButton>()); }
                    buttons[buttons.Count - 1].Add
                        (InlineKeyboardButton.WithCallbackData(text: c.Name, callbackData: $"CreateSubCategory {c.Id} {idEntity}"));
                    i++;
                }

                InlineKeyboardMarkup inlines = new InlineKeyboardMarkup(buttons);
                return inlines;
            }
        }
        /// <summary>
        /// Район
        /// </summary>
        /// <returns></returns>
        public static async Task<InlineKeyboardMarkup> District(int idEntity, string role, int ?idCity)
        {
            using (FLBotContext botContext = new FLBotContext())
            {
                var districts = await botContext.Districts.Where(a => a.IdCity == idCity).ToListAsync();

                List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

                int i = 0;
                foreach (var c in districts)
                {
                    if(i % 3 == 0) { buttons.Add(new List<InlineKeyboardButton>()); }
                    buttons[buttons.Count - 1].Add
                        (InlineKeyboardButton.WithCallbackData(text: c.Name, callbackData: $"CreateDistrict {c.Id} {idEntity}"));

                    i++;
                }
                if (role == "Работник")
                {
                    buttons.Add(new List<InlineKeyboardButton>());
                    buttons[buttons.Count - 1].Add
                     (InlineKeyboardButton.WithCallbackData(text: "Все районы", callbackData: $"CreateDistrict 0 {idEntity}"));

                }
                InlineKeyboardMarkup inlines = new(buttons);

                return inlines;
            }
        }
        /// <summary>
        /// Публикация заказа
        /// </summary>
        /// <param name="idOrder"></param>
        /// <returns></returns>
        public static InlineKeyboardMarkup GetOrder(int idOrder)
        {           
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
                
            buttons.Add(InlineKeyboardButton.WithCallbackData(text: "Опубликовать", callbackData: $"CreatePublish empty {idOrder}"));               

            InlineKeyboardMarkup inlines = new InlineKeyboardMarkup(buttons);
            return inlines;            
        }
        /// <summary>
        /// Публикация фильтра
        /// </summary>
        /// <param name="idFilter"></param>
        /// <returns></returns>
        public static InlineKeyboardMarkup GetFilter(int idFilter)
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            buttons.Add(InlineKeyboardButton.WithCallbackData(text: "Найти работу", callbackData: $"CreatePublish empty {idFilter}"));
            buttons.Add(AddFavoriteFilter(idFilter));

            InlineKeyboardMarkup inlines = new InlineKeyboardMarkup(buttons);
            return inlines;
        }
        /// <summary>
        /// Отклик на заказ
        /// </summary>
        /// <param name="idOrder"></param>
        /// <returns></returns>
        public static InlineKeyboardMarkup Response(int idOrder)
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            buttons.Add(InlineKeyboardButton.WithCallbackData(text: "Откликнуться", callbackData: $"Response {idOrder}"));

            InlineKeyboardMarkup inlines = new InlineKeyboardMarkup(buttons);
            return inlines;
        }
        /// <summary>
        /// Договор с сотрудником
        /// </summary>
        /// <param name="idOrder"></param>
        /// <returns></returns>
        public static InlineKeyboardMarkup Agreed(int idOrder)
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            buttons.Add(InlineKeyboardButton.WithCallbackData(text: "Договорились", callbackData: $"Agreed {idOrder}"));

            InlineKeyboardMarkup inlines = new InlineKeyboardMarkup(buttons);
            return inlines;
        }
        /// <summary>
        /// Архив заказа
        /// </summary>
        /// <param name="idOrder"></param>
        /// <returns></returns>
        public static InlineKeyboardMarkup ArchiveOrder(int idOrder)
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            buttons.Add(InlineKeyboardButton.WithCallbackData(text: "В архив", callbackData: $"ArchiveOrder {idOrder}"));

            InlineKeyboardMarkup inlines = new InlineKeyboardMarkup(buttons);
            return inlines;
        }
        /// <summary>
        /// Архив фильтра
        /// </summary>
        /// <param name="idFilter"></param>
        /// <returns></returns>
        public static InlineKeyboardMarkup ArchiveFilter(int idFilter)
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            buttons.Add(InlineKeyboardButton.WithCallbackData(text: "В архив", callbackData: $"ArchiveFilter {idFilter}"));

            InlineKeyboardMarkup inlines = new InlineKeyboardMarkup(buttons);
            return inlines;
        }
        /// <summary>
        /// Добавление заказа в избранное
        /// </summary>
        /// <param name="idOrder"></param>
        /// <returns></returns>
        public static InlineKeyboardMarkup AddFavoriteOrder(int idOrder)
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            buttons.Add(InlineKeyboardButton.WithCallbackData(text: "Сохранить", callbackData: $"AddFavoriteOrder {idOrder}"));

            InlineKeyboardMarkup inlines = new InlineKeyboardMarkup(buttons);
            return inlines;
        }
        /// <summary>
        /// Добавление заказа в избранное
        /// </summary>
        /// <param name="idFilter"></param>
        /// <returns></returns>
        public static InlineKeyboardButton AddFavoriteFilter(int idFilter)
        {
            return InlineKeyboardButton.WithCallbackData(text: "Сохранить", callbackData: $"AddFavoriteFilter {idFilter}");
        }
        /// <summary>
        /// Добавить новый или выбрать избранный
        /// </summary>
        /// <returns></returns>
        public static InlineKeyboardMarkup FavoriteOrNewOrder()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            buttons.Add(InlineKeyboardButton.WithCallbackData(text: "Новый заказ", callbackData: $"NewOrder"));
            buttons.Add(InlineKeyboardButton.WithCallbackData(text: "Избранные заказы", callbackData: $"FavoriteOrders"));

            InlineKeyboardMarkup inlines = new InlineKeyboardMarkup(buttons);
            return inlines;
        }
        /// <summary>
        /// Добавить новый или выбрать избранный
        /// </summary>
        /// <returns></returns>
        public static InlineKeyboardMarkup FavoriteOrNewFilter()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            buttons.Add(InlineKeyboardButton.WithCallbackData(text: "Новый фильтр", callbackData: $"NewFilter"));
            buttons.Add(InlineKeyboardButton.WithCallbackData(text: "Избранные фильтры", callbackData: $"FavoriteFilters"));

            InlineKeyboardMarkup inlines = new InlineKeyboardMarkup(buttons);
            return inlines;
        }
    }
}
