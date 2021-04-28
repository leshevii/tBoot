using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace lesson9_telegram_bot
{
    public class MyBoot
    {
        private ITelegramBotClient tBootClient;
        /// <summary>
        /// Путь к файлу с токеном
        /// </summary>
        private string path;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="tBootClient">экземпляр ITelegramBotClient</param>
        /// <param name="path">Путь к файлу с токеном для telegram api</param>
        public MyBoot(ITelegramBotClient tBootClient,string path)
        {
            this.tBootClient = tBootClient;
            this.path        = path;
            
        }
        /// <summary>
        /// Позволяет добавить команды к боту
        /// </summary>
        /// <param name="commands">List<BotCommand> список комманд</param>
        public void AddBotCommand(List<BotCommand>commands)
        {
            tBootClient.SetMyCommandsAsync(commands);
        }
        /// <summary>
        /// Обработчик updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        public async void OnMessage(object sender, MessageEventArgs arg)
        {            
            bool status;
            switch (arg.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Text:
                    if (IsCommand(arg.Message.Text).Result)
                    {
                        
                        if (arg.Message.Text == "/list")
                            PrintAllFiles(arg.Message.Chat);
                        if (arg.Message.Text == "/weather")
                            SendText(arg.Message.Chat, "send your geo position");
                        else
                            PrintInfo(arg.Message.Chat);
                    }
                    else
                    {
                        SendText(arg.Message.Chat, "Hello");
                    }
                break;
                case Telegram.Bot.Types.Enums.MessageType.Document:
                     saveFile(arg.Message.Document.FileId, Telegram.Bot.Types.Enums.MessageType.Document.ToString());                     
                break;
                case Telegram.Bot.Types.Enums.MessageType.Photo:
                     PhotoSize[] ps = arg.Message.Photo;                    
                     status = saveFile(ps[0].FileId, Telegram.Bot.Types.Enums.MessageType.Photo.ToString());                     
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Audio:                    
                     status = saveFile(arg.Message.Audio.FileId, Telegram.Bot.Types.Enums.MessageType.Audio.ToString());                     
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Voice:
                    status = saveFile(arg.Message.Voice.FileId, Telegram.Bot.Types.Enums.MessageType.Voice.ToString());                    
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Animation:
                    status = saveFile(arg.Message.Animation.FileId, Telegram.Bot.Types.Enums.MessageType.Animation.ToString());
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Location:                    
                    PrintInfoWeather(arg.Message.Chat, MainClass.weatherToken, arg.Message.Location.Longitude, arg.Message.Location.Latitude);
                    break;
                default:
                        SendText(arg.Message.Chat, "Not save");
                    break;
            }
        }
        /// <summary>
        /// Печать всех загруженных файлов в Telegram
        /// </summary>
        /// <param name="chat"></param>
        public async void PrintAllFiles(Chat chat)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
                {                   
                   var arr =  sr.ReadLine().Trim().Split(',');
                    while (!sr.EndOfStream)
                    {
                        arr = sr.ReadLine().Trim().Split(',');
                        SendAnyFile(chat, arr[0], arr[1]);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading file");
            }
        }
        /// <summary>
        /// Являеться ли коммандой переданное собщении
        /// </summary>
        /// <param name="text">текст сообщения</param>
        /// <returns></returns>
        public async Task<bool> IsCommand(string text)
        {
            Console.WriteLine(text);
            var commands = await tBootClient.GetMyCommandsAsync();

            foreach(var command in commands)
            {
                Console.WriteLine(command.Command);
                if ('/'+command.Command == text) return true;
            }
            return false;
        }
        /// <summary>
        /// Отправляет любой тип файла в качестве сообщения
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="fileId">Id файла хранящегося на серверах Telegram</param>
        /// <param name="type">тип файла</param>
        public async void SendAnyFile(Chat chat,string fileId,string type)
        {
            InputOnlineFile inputOnlineFile = new InputOnlineFile(fileId);
            if (type == Telegram.Bot.Types.Enums.MessageType.Photo.ToString())                            
                await tBootClient.SendPhotoAsync(chat, inputOnlineFile);                
            else if(type == Telegram.Bot.Types.Enums.MessageType.Document.ToString())
                await tBootClient.SendDocumentAsync(chat, inputOnlineFile);
            else if (type == Telegram.Bot.Types.Enums.MessageType.Voice.ToString())
                await tBootClient.SendVoiceAsync(chat, inputOnlineFile);
            else if (type == Telegram.Bot.Types.Enums.MessageType.Animation.ToString())
                await tBootClient.SendAnimationAsync(chat, inputOnlineFile);
            else if (type == Telegram.Bot.Types.Enums.MessageType.Audio.ToString())
                await tBootClient.SendAnimationAsync(chat, inputOnlineFile);
        }
        /// <summary>
        /// Отправляет текстовое сообщение
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="text"></param>
        public async void SendText(Chat chat, string text)
        {
            await tBootClient.SendTextMessageAsync
                (
                     chatId: chat,
                     text:  text                     
                );
        }
        /// <summary>
        /// выводит список команд на старте бота
        /// </summary>
        /// <param name="chat"></param>
        public async void PrintInfo(Chat chat)
        {
            var str = string.Empty;
            BotCommand[] botCommand =  await tBootClient.GetMyCommandsAsync();
            List<KeyboardButton> kb = new List<KeyboardButton>();
            str += "*Your Commands:* \n";
            foreach (BotCommand b in botCommand)
            {
                str += '/' + b.Command.ToString() + " \\-" + b.Description.ToString()+ '\n';
                if (b.Command.ToString() == "weather")
                {
                    KeyboardButton kb2 = new KeyboardButton('/' + b.Command.ToString());
                    kb2.RequestLocation = true;
                    kb.Add(kb2);
                }
                else
                    kb.Add(new KeyboardButton('/' + b.Command.ToString()));
            }                        
            ReplyKeyboardMarkup rp = new ReplyKeyboardMarkup(kb);
            try
            {
                await tBootClient.SendTextMessageAsync
                    (
                    chatId: chat,
                    text: str,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                    replyMarkup: rp
                    );
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// Выводит информацию о погоде для переданной геопозиции
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="path">путь к токену для сайта weather</param>
        /// <param name="lon">долгота</param>
        /// <param name="lat">широта</param>
        public async void PrintInfoWeather(Chat chat,string path,float lon,float lat)
        {
            Weather weather = new Weather(path, lon,lat);            
            try
            {
                await tBootClient.SendTextMessageAsync
                    (
                    chatId: chat,
                    text: weather.GetCurrentCityName() + " - " + weather.GetCurrentTemperature(Weather.Units.Celsius)
                    );
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// Сохраняет fileId в текстовый файл
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool saveFile(string fileId, string type)
        {
            string str = string.Empty;
            Console.WriteLine(path);
            try
            {
                using (StreamWriter sw = new StreamWriter(path, append: true, System.Text.Encoding.UTF8))
                {
                    str = fileId + ',' + type;
                    sw.WriteLine(str);
                }
            }catch(Exception e)
            {
                return false;
            }
            
            return true;
        }       
    }
}
