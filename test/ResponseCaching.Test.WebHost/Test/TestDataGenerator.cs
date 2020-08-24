using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.WebHost.Test
{
    public static class TestDataGenerator
    {
        private static readonly string[] _summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly Random _random = new Random();

        public const int Count = 25;

        public static IEnumerable<WeatherForecast> GetData()
        {
            return Enumerable.Range(1, Count).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = _random.Next(-20, 55),
                Summary = _summaries[_random.Next(_summaries.Length)]
            }).ToArray();
        }

        public static IEnumerable<WeatherForecast> GetData(int count)
        {
            return GetData(0, count);
        }

        public static IEnumerable<WeatherForecast> GetData(int skip, int count)
        {
            return Enumerable.Range(skip + 1, count).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = _random.Next(-20, 55),
                Summary = _summaries[_random.Next(_summaries.Length)]
            }).ToArray();
        }
    }
}