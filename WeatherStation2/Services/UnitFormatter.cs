namespace WeatherStation2.Services;

// Forecasts are always stored in Celsius and km/h. This class converts them for display
// using the user's settings, so changing a unit never invalidates the cache.
// Pages call Temp, TempDiff and Wind whenever they need to show a number as text.
public class UnitFormatter
{
    // Where the user's chosen units come from.
    private readonly SettingsService _settings;

    // Dependency injection: MAUI passes in the SettingsService when it creates this class.
    public UnitFormatter(SettingsService settings) => _settings = settings;

    // True when the user picked Fahrenheit. It reads the setting each time, so a change applies straight away.
    private bool IsFahrenheit => _settings.Current.TemperatureUnit == "Fahrenheit";

    // Turns a Celsius temperature into display text such as "18°".
    // The double? type means the value may be missing (null). Missing data shows as "--".
    public string Temp(double? celsius)
    {
        if (celsius == null) return "--";
        // Fahrenheit = Celsius x 9/5 + 32. Otherwise leave the Celsius value as it is.
        // .Value gets the real number out of the nullable type (safe here because we checked for null).
        var value = IsFahrenheit ? celsius.Value * 9 / 5 + 32 : celsius.Value;
        return $"{Round(value)}°";
    }

    // For "X is 5° warmer" text: a difference needs no +32 offset.
    // It returns a positive size only, because the caller words it as warmer or cooler.
    public string TempDiff(double celsiusDifference)
    {
        // Only the x 9/5 scaling applies to a gap between two temperatures. Math.Abs drops any minus sign.
        var value = Math.Abs(IsFahrenheit ? celsiusDifference * 9 / 5 : celsiusDifference);
        return $"{Round(value)}°";
    }

    // Turns a wind speed in km/h into display text in the user's chosen unit. Missing data shows as "--".
    public string Wind(double? kmh)
    {
        if (kmh == null) return "--";
        // A switch expression picks the unit. Conversion factors: 1 km/h = 0.621371 mph, and divide by 3.6 for m/s.
        return _settings.Current.WindUnit switch
        {
            "mph" => $"{Round(kmh.Value * 0.621371)} mph",
            "m/s" => $"{Round(kmh.Value / 3.6)} m/s",
            _ => $"{Round(kmh.Value)} km/h"
        };
    }

    // Rounds to a whole number and avoids showing "-0".
    // Math.Round(-0.3) is -0, which would print as "-0°".
    private static double Round(double value)
    {
        var rounded = Math.Round(value);
        // Comparing with 0 is true for both 0 and -0, and returning plain 0 gives the positive version.
        return rounded == 0 ? 0 : rounded;
    }
}
