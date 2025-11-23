using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.WebHost.Test;

public static class TestDataGenerator
{
    public const int Count = 25;

    private static readonly string[] s_summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public static IEnumerable<WeatherForecast> GetData(int count)
    {
        return GetData(0, count);
    }

    public static IEnumerable<WeatherForecast> GetData(int skip = 0, int count = Count)
    {
        var random = Random.Shared;
        return Enumerable.Range(skip + 1, count).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = random.Next(-20, 55),
            Summary = s_summaries[random.Next(s_summaries.Length)]
        }).ToArray();
    }
}
