using System.Globalization;
using System.Text.Json.Serialization;
using WeatherStation2.Helpers;

namespace WeatherStation2.Models;

public class ForecastResponse
{
    [JsonPropertyName("timezone")] public string? Timezone { get; set; }
    [JsonPropertyName("current")] public CurrentWeather? Current { get; set; }
    [JsonPropertyName("daily")] public DailyData? Daily { get; set; }
}

public class CurrentWeather
{
    [JsonPropertyName("temperature_2m")] public double? Temperature { get; set; }
    [JsonPropertyName("relative_humidity_2m")] public int? Humidity { get; set; }
    [JsonPropertyName("apparent_temperature")] public double? FeelsLike { get; set; }
    [JsonPropertyName("precipitation")] public double? Precipitation { get; set; }
    [JsonPropertyName("weather_code")] public int? WeatherCode { get; set; }
    [JsonPropertyName("wind_speed_10m")] public double? WindSpeed { get; set; }

    [JsonIgnore] public string Icon => WeatherIcons.FromCode(WeatherCode);
}

// Open-Meteo returns "parallel arrays": index 0 of every list is day 1, index 1 is day 2, and so on.
public class DailyData
{
    [JsonPropertyName("time")] public List<string>? Time { get; set; }
    [JsonPropertyName("weather_code")] public List<int?>? WeatherCode { get; set; }
    [JsonPropertyName("temperature_2m_max")] public List<double?>? TempMax { get; set; }
    [JsonPropertyName("temperature_2m_min")] public List<double?>? TempMin { get; set; }
    [JsonPropertyName("precipitation_probability_max")] public List<int?>? RainChance { get; set; }
    [JsonPropertyName("wind_speed_10m_max")] public List<double?>? WindMax { get; set; }

    // Flatten the parallel arrays into one object per day so the UI can bind to a list.
    public List<DayForecast> ToDays()
    {
        var days = new List<DayForecast>();
        if (Time == null) return days;

        for (int i = 0; i < Time.Count; i++)
        {
            days.Add(new DayForecast
            {
                Date = DateTime.TryParse(Time[i], CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) ? d : DateTime.MinValue,
                WeatherCode = WeatherCode?.ElementAtOrDefault(i),
                TempMax = TempMax?.ElementAtOrDefault(i),
                TempMin = TempMin?.ElementAtOrDefault(i),
                RainChance = RainChance?.ElementAtOrDefault(i),
                WindMax = WindMax?.ElementAtOrDefault(i)
            });
        }
        return days;
    }
}

public class DayForecast
{
    public DateTime Date { get; set; }
    public int? WeatherCode { get; set; }
    public double? TempMax { get; set; }
    public double? TempMin { get; set; }
    public int? RainChance { get; set; }
    public double? WindMax { get; set; }

    [JsonIgnore] public string Icon => WeatherIcons.FromCode(WeatherCode);
}
