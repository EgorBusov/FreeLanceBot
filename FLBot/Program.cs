using FLBot.Data;
using FLBot.Models;
using FLBot.Telegram;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

internal class Program
{
    static TelegramBotClient Bot = BotClient.botClient;
    /// <summary>
    /// Коллекция статусов клиентов
    /// </summary>
    private static readonly ConcurrentDictionary<long, UserState> _userStates = new ConcurrentDictionary<long, UserState>();
    private static async Task Main(string[] args)
    {
        while (true)
        {
            try
            {
                DataMethods.ActualOrders += Messages.SendOrders;
                DataMethods.ActualFilters += Messages.SendOrders;
                Bot.StartReceiving(UpdateAsync, ErrorAsync);
                Console.ReadKey();
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Exception: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }
    }
    public static async Task UpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        string pattern; // для проверки текста с помощью регулярных выражения
        UserState userState; // для определения статуса пользователя
        ///Обработка текстового сообщения
        if (update.Type == UpdateType.Message)
        {           
            var message = update.Message;
            userState = _userStates.GetOrAdd(message.From.Id, new UserState());
            if (userState.State == null)
            {
                switch (message.Text)
                {
                    case "/start":
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            text: Messages.HelloMessage(message.From.FirstName),
                            replyMarkup: Keyboards.Role());
                        await DataMethods.AddClient(message.From.Id, message.From.FirstName,
                                                    message.From.LastName, message.From.Username, message.Chat.Id);
                        break;
                    case "/myorders":
                        await Messages.ActualOrders(message.From.Id, botClient, message.Chat.Id);
                        break;
                    case "/myfilters":
                        await Messages.ActualFilters(message.From.Id, botClient, message.Chat.Id);
                        break;
                    default:
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            text: "Я такого к сожалению не умею. Выберите из предложенного",
                            replyMarkup: Keyboards.Role());
                        break;
                }
            }
            else //userState.State != null
            {
                switch(userState.State) 
                {
                    case "AddAddress":
                        await DataMethods.AddAddressOrder(message.From.Id, userState.IdOrder, message.Text);
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            text: "Введите график работы. Пример:\"9.00-21.00\"");
                        userState.State = "AddSchedule";
                        break;
                    case "AddSchedule":
                        await DataMethods.AddScheduleOrder(message.From.Id, userState.IdOrder, message.Text);
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            text: "Введите размер оплаты за 1 смену. Пример:\"2500 руб\"");
                        userState.State = "AddPayment";
                        break;
                    case "AddPayment":
                        await DataMethods.AddPaymentOrder(message.From.Id, userState.IdOrder, message.Text);
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            text: "Добавьте описание. Пример:\"доставлять суши\"");
                        userState.State = "AddDescription";
                        break;
                    case "AddDescription":
                        await DataMethods.AddDescriptionOrder(message.From.Id, userState.IdOrder, message.Text);
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            text: await Messages.OrderMessage(userState.IdOrder),
                            replyMarkup: Keyboards.GetOrder(userState.IdOrder));
                        userState.State = "Publish";
                        break;
                    case "AddName":
                        pattern = @"^[а-яА-Я]+$";

                        if (Regex.IsMatch(message.Text, pattern) && message.Text != null)
                        {
                            await DataMethods.AddNameFilter(message.From.Id, message.Text, userState.IdFilter);
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                text: "Введите номер телефона для связи. Пример: +79509509509");
                            userState.State = "AddPhone";
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                text: "Имя введено некорректно. Пример: Иван"
                                );
                        }                       
                        break;
                    case "AddPhone":
                        pattern = @"^(\+7|8)\d{10}$";

                        if (Regex.IsMatch(message.Text, pattern))
                        {
                            await DataMethods.AddPhoneFilter(message.From.Id, message.Text, userState.IdFilter);
                            userState.State = null;
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                text: await Messages.FilterMessage(userState.IdFilter),
                                replyMarkup: Keyboards.GetFilter(userState.IdFilter)
                                );
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                text: "Номер телефона введен некорректно. Пример: +79509509509"
                                );
                        }
                        break;
                }
            }

        }

        ///Обработка нажатий на кнопки
        if (update.Type == UpdateType.CallbackQuery)
        {
            int idOrder; //id заказа, который возвращается и принимается в каждом методе
            var callbackQuery = update.CallbackQuery;
            userState = _userStates.GetOrAdd(callbackQuery.From.Id, new UserState());
            List<string> splitAnswer = callbackQuery.Data.Split(' ').ToList(); //в callbackdata передаются еще некоторые данные.
            switch (splitAnswer[0]) //Обработка нажатий вне зависимости от роли
            {

                case "employer":                            //Roles работодатель
                    await botClient.SendTextMessageAsync(
                        callbackQuery.Message.Chat.Id,
                        text: "Можете создать новый или выбрать избранный, если он есть.",
                        replyMarkup: Keyboards.FavoriteOrNewOrder()
                        );
                    userState.Role = "Работодатель";
                    break;
                case "employee":                            //Roles сотрудник
                    await botClient.SendTextMessageAsync(
                        callbackQuery.Message.Chat.Id,
                        text: "Можете создать новый или выбрать избранный, если он есть.",
                        replyMarkup: Keyboards.FavoriteOrNewFilter()
                        );
                    userState.Role = "Работник";
                    break;                               
                case "Response":
                    await Messages.ResponseMessage(int.Parse(splitAnswer[1]),callbackQuery.From.Id, botClient);
                    await botClient.SendTextMessageAsync(
                        callbackQuery.Message.Chat.Id,
                        text: "Работодатель получил ваши данные и свяжется с вами в ближайшее время"
                        );
                    break;
                case "Agreed":
                    await DataMethods.ArchiveOrder(int.Parse(splitAnswer[1]));
                    await botClient.SendTextMessageAsync(
                        callbackQuery.Message.Chat.Id,
                        text: $"Ваша заявка переведена в архив. Id заявки: {splitAnswer[1]}"
                        );
                    break;
                case "ArchiveOrder":
                    await DataMethods.ArchiveOrder(int.Parse(splitAnswer[1]));
                    await botClient.SendTextMessageAsync(
                        callbackQuery.Message.Chat.Id,
                        text: $"Ваша заявка переведена в архив. Id заявки: {splitAnswer[1]}"
                        );
                    break;
                case "ArchiveFilter":
                    await DataMethods.ArchiveFilter(int.Parse(splitAnswer[1]));
                    await botClient.SendTextMessageAsync(
                        callbackQuery.Message.Chat.Id,
                        text: $"Ваш фильтр переведен в архив. Id фильтра: {splitAnswer[1]}"
                        );
                    break;
            }

            if (userState.Role == "Работодатель") //Обработка нажатий работодателя
            {
                switch (splitAnswer[0])
                {
                    case "FavoriteOrders":
                        await Messages.SendFavoriteOrders(callbackQuery.From.Id, botClient, callbackQuery.Message.Chat.Id);
                        break;
                    case "NewOrder":
                        await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id,
                            text: "Выберите город",
                            replyMarkup: await Keyboards.Cities()
                            );
                        break;
                    case "CreateCity":
                        idOrder = await DataMethods.AddNewOrder(callbackQuery.From.Id, int.Parse(splitAnswer[1]));
                        await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id,
                            text: "Выберите категорию",
                            replyMarkup: await Keyboards.Categories(idOrder)
                            );
                        userState.IdOrder = idOrder;
                        break;
                    case "CreateCategory":
                        bool check = await DataMethods.AddCategoryOrder
                            (callbackQuery.From.Id, userState.IdOrder, int.Parse(splitAnswer[1]));
                        if (check)
                        {
                            await botClient.SendTextMessageAsync(
                                callbackQuery.Message.Chat.Id,
                                text: "Выберите подкатегорию",
                                replyMarkup: await Keyboards.SubCategories(userState.IdOrder, int.Parse(splitAnswer[1]))
                                );
                        }
                        else
                        {
                            await Messages.AddDistrict
                                (userState.IdOrder, botClient, callbackQuery.Message.Chat.Id, userState.Role);
                        }
                        break;
                    case "CreateSubCategory":

                        await DataMethods.AddSubCategoryOrder(callbackQuery.From.Id, userState.IdOrder, int.Parse(splitAnswer[1]));
                        await Messages.AddDistrict(userState.IdOrder, botClient, callbackQuery.Message.Chat.Id, userState.Role);
                        break;
                    case "CreateDistrict":
                        idOrder = await DataMethods.AddDistrictOrder(callbackQuery.From.Id, userState.IdOrder, int.Parse(splitAnswer[1]));
                        await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id,
                            text: "Введите адрес. Пример: \"Герцена 80\""
                            );
                        userState.State = $"AddAddress";
                        userState.IdOrder = idOrder;
                        break;
                    case "CreatePublish":
                        idOrder = await DataMethods.PublishOrder(Convert.ToInt32(splitAnswer[2]), botClient);
                        if (idOrder != 0)
                        {
                            await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id,
                            text: $"Ваша заявка опубликована. Id заявки: {idOrder} \n" +
                            "Ожидайте откликов.",
                            replyMarkup: Keyboards.AddFavoriteOrder(idOrder)
                            );
                            userState.State = null;
                        }
                        break;
                    case "AddFavoriteOrder":
                        await DataMethods.AddFavotitesOrder(int.Parse(splitAnswer[1]));
                        await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id,
                            text: $"Ваша заявка добавлена в избранное. Id заявки: {int.Parse(splitAnswer[1])}"
                            );
                        break;

                }
            }

            if(userState.Role == "Работник") //Обработка нажатий работника
            {
                switch(splitAnswer[0])
                {
                    case "FavoriteFilters":
                        await Messages.SendFavoriteFilters(callbackQuery.From.Id, botClient, callbackQuery.Message.Chat.Id);
                        break;
                    case "NewFilter":
                        await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id,
                            text: "Выберите город",
                            replyMarkup: await Keyboards.Cities()
                            );
                        break;
                    case "CreateCity":
                        int idFilter = await DataMethods.AddNewFilter
                            (callbackQuery.From.Id, int.Parse(splitAnswer[1]), callbackQuery.Message.Chat.Id);
                        await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id,
                            text: "Выберите категорию",
                            replyMarkup: await Keyboards.Categories(idFilter)
                            );
                        userState.IdFilter = idFilter;
                        break;
                    case "CreateCategory":
                        bool check = await DataMethods.AddCategoryFilter
                            (callbackQuery.From.Id, userState.IdFilter, int.Parse(splitAnswer[1]));
                        if (check)
                        {
                            await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id,
                            text: "Выберите подкатегорию",
                            replyMarkup: await Keyboards.SubCategories(userState.IdFilter, int.Parse(splitAnswer[1]))
                            );
                        }
                        else
                        {
                            await Messages.AddDistrict
                                (userState.IdFilter, botClient, callbackQuery.Message.Chat.Id, userState.Role);
                        }                       
                        break;
                    case "CreateSubCategory":
                        await DataMethods.AddSubCategoryFilter(callbackQuery.From.Id, userState.IdFilter, int.Parse(splitAnswer[1]));
                        await Messages.AddDistrict(userState.IdFilter, botClient, callbackQuery.Message.Chat.Id, userState.Role);
                        break;
                    case "CreateDistrict":
                        idFilter = await DataMethods.AddDistrictFilter(callbackQuery.From.Id, userState.IdFilter, int.Parse(splitAnswer[1]));
                        await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id,
                            text: "Введите имя. Пример: \"Иван\"."
                            );
                        userState.State = $"AddName";
                        userState.IdFilter = idFilter;
                        break;
                    case "CreatePublish":
                        await DataMethods.PublishFilter(callbackQuery.From.Id, callbackQuery.Message.Chat.Id,
                                                                                            Convert.ToInt32(splitAnswer[2]), botClient);
                        userState.State = null;
                        break;
                    case "AddFavoriteFilter":
                        await DataMethods.AddFavotitesFilter(int.Parse(splitAnswer[1]));
                        await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id,
                            text: $"Ваш фильтр добавлен в избранное. Id фильтра: {int.Parse(splitAnswer[1])}"
                            );
                        break;
                }
            }
        }
    }
    /// <summary>
    /// Обработка ошибок в боте
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="exception"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task ErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
    {
        Console.WriteLine(exception.Message);
    }
}