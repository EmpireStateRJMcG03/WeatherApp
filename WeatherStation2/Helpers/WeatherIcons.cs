namespace WeatherStation2.Helpers;

public static class WeatherIcons
{
    // Turns an Open-Meteo weather code into the name of an image in Resources/Images.
    public static string FromCode(int? code) => code switch
    {
        0 => "clear_day",
        1 or 2 or 3 => "cloudy",
        45 or 48 => "fog",
        >= 51 and <= 57 => "drizzle",
        >= 61 and <= 67 or >= 80 and <= 82 => "rain",
        >= 71 and <= 77 or 85 or 86 => "snow",
        >= 95 and <= 99 => "thunder",
        _ => "unknown"   // missing or unexpected code
    };

    // Turns the same code into words for the user.
    public static string Describe(int? code) => code switch
    {
        0 => "Clear sky",
        1 => "Mainly clear",
        2 => "Partly cloudy",
        3 => "Overcast",
        45 or 48 => "Fog",
        51 or 53 or 55 => "Drizzle",
        56 or 57 => "Freezing drizzle",
        61 => "Light rain",
        63 => "Rain",
        65 => "Heavy rain",
        66 or 67 => "Freezing rain",
        71 => "Light snow",
        73 => "Snow",
        75 => "Heavy snow",
        77 => "Snow grains",
        80 => "Light showers",
        81 => "Showers",
        82 => "Heavy showers",
        85 or 86 => "Snow showers",
        95 => "Thunderstorm",
        96 or 99 => "Thunderstorm with hail",
        _ => "Unknown"
    };
}

