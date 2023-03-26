using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ResponseCaching.Test.WebHost.Models;

#pragma warning disable CS0660 // 类型定义运算符 == 或运算符 !=，但不重写 Object.Equals(object o)
public class WeatherForecast : IEquatable<WeatherForecast>
#pragma warning restore CS0660 // 类型定义运算符 == 或运算符 !=，但不重写 Object.Equals(object o)
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string Summary { get; set; }

    public bool Equals([AllowNull] WeatherForecast other)
    {
        return other != null
               & other.Date == Date
               & other.Summary == Summary
               & other.TemperatureC == TemperatureC;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Date, TemperatureC, Summary);
    }

    public static bool operator ==(WeatherForecast left, WeatherForecast right)
    {
        return EqualityComparer<WeatherForecast>.Default.Equals(left, right);
    }

    public static bool operator !=(WeatherForecast left, WeatherForecast right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"{Date},{Summary},{TemperatureC}";
    }
}