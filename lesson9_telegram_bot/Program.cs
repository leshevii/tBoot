using System;
using System.IO;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace lesson9_telegram_bot
{
    class MainClass
    {
        /// <summary>
        /// Токен для telegram bot
        /// </summary>
        public const string token = "/Users/ivanlesev/Projects/lesson9_telegram_bot/lesson9_telegram_bot/bin/Debug/token.txt";
        /// <summary>
        /// Токен для openweathermap.org
        /// </summary>
        public const string weatherToken = "/Users/ivanlesev/Projects/lesson9_telegram_bot/lesson9_telegram_bot/bin/Debug/wToken2.txt";

        static ITelegramBotClient bot;                
        static List<BotCommand> bCommand = new List<BotCommand>()
        {
            new BotCommand()
            {
                Command     = "/start",
                Description = "About bot"
            },
            new BotCommand()
            {
                Command     = "/list",
                Description = "list your uploading files"
            },
            new BotCommand()
            {
                Command     = "/weather",
                Description = "Weather current your Town"
            },
        };
        

        public static void Main(string[] args)
        {
            try
            {
               var locToken = System.IO.File.ReadAllText(token);
                if (!string.IsNullOrEmpty(locToken))
                {
                    bot = new TelegramBotClient(locToken);
                    
                    MyBoot myBoot = new MyBoot(bot, Directory.GetCurrentDirectory() + "/res.txt");
                    myBoot.AddBotCommand(bCommand);

                    bot.OnMessage += myBoot.OnMessage;
                    bot.StartReceiving();
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Файл не содержит токен");
                    Console.ReadKey();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Проблемы с открытием файла");
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
            
        }
        
    }
}
