using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace lesson9_telegram_bot
{
    public class Weather
    {
        public enum Units { Fahrenheit, Celsius }
        public float latitude;
        public float longitude;

        private WebClient wb;
        private string    path;
        private string    token;

        public Weather(string path, float longitude, float latitude)
        {
            wb = new WebClient() { Encoding = Encoding.UTF8};            
            this.path      = path;
            this.longitude = longitude;
            this.latitude  = latitude;

            try
            {
                token = File.ReadAllText(path);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Возвращает Название города
        /// </summary>
        /// <returns></returns>
        public string GetCurrentCityName()
        {                      
            var str = wb.DownloadString($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&lang=ru&appid={token}");
            JObject jObject = JObject.Parse(str);
            str = (string)jObject["name"];

            return str;
        }
        /// <summary>
        /// Возвращает температуру
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public string GetCurrentTemperature(Units units)
        {            
            string unit;

            if (Units.Celsius == units)
                unit = "metric";
            else
                unit = "imperial";

            var str = wb.DownloadString($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&units={unit}&appid={token}");
            JObject jObject = JObject.Parse(str);
            str = (string)jObject["main"]["temp"];
            
            return str;
        }
    }
}
