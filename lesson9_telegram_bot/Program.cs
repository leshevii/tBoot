using System;
using System.IO;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace lesson9_telegram_bot
{
    class MainClass
    {
        static ITelegramBotClient bot;        
        static string token;
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
            token = System.IO.File.ReadAllText("/Users/ivanlesev/token.txt");
            bot = new TelegramBotClient(token);

            MyBoot myBoot = new MyBoot(bot,Directory.GetCurrentDirectory()+"/res.txt");
            myBoot.AddBotCommand(bCommand);

            bot.OnMessage += myBoot.OnMessage;
            bot.StartReceiving();            
            Console.ReadKey();
        }
        
    }
}
