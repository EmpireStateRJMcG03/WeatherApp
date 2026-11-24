using System.Globalization;
using System.Net.Http.Json;
using WeatherStation2.Models;
using Location = WeatherStation2.Models.Location;

namespace WeatherStation2.Services;

// Talks to the Open-Meteo forecast API. It only fetches; caching lives in ForecastRepository.
// ForecastRepository calls this class when it decides it needs fresh data from the internet.
// Open-Meteo is free and needs no API key.
public class WeatherService
{
    // One shared HttpClient for the whole app. It is static so every request reuses it.
    // Creating a new HttpClient each time wastes network connections, so one shared copy is best practice.
    // The 10 second timeout stops the app waiting forever on a bad connection.
    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(10) };

    // Downloads the forecast for one location.
    // Returns the parsed forecast, or null if the request failed (no internet, timeout, bad data).
    // The CancellationToken lets the caller cancel the request part-way. The default means "no cancelling".
    public async Task<ForecastResponse?> GetForecastAsync(Location location, CancellationToken ct = default)
    {
        // InvariantCulture forces a dot as the decimal point (53.35). Without it, a phone set to a language
        // like German or French would write a comma (53,35) and the web address would break.
        var lat = location.Latitude.ToString(CultureInfo.InvariantCulture);
        var lon = location.Longitude.ToString(CultureInfo.InvariantCulture);

        // Build the web address. The part after the question mark lists what we are asking for:
        // "current" is the weather right now, "daily" is the day-by-day forecast.
        // timezone=auto gives times in the location's own time zone. forecast_days=16 asks for 16 days.
        var url = "https://api.open-meteo.com/v1/forecast" +
                  $"?latitude={lat}&longitude={lon}" +
                  "&current=temperature_2m,relative_humidity_2m,apparent_temperature," +
                  "precipitation,weather_code,wind_speed_10m" +
                  "&daily=weather_code,temperature_2m_max,temperature_2m_min," +
                  "precipitation_probability_max,wind_speed_10m_max" +
                  "&timezone=auto&forecast_days=16";

        try
        {
            // await pauses this method until the download finishes, without freezing the screen.
            // GetFromJsonAsync downloads the JSON and converts it into a ForecastResponse object in one step.
            return await _http.GetFromJsonAsync<ForecastResponse>(url, ct);
        }
        // Expected failures: network or HTTP error, timeout or cancel (both appear as TaskCanceledException),
        // JSON that does not match our model, or an unsupported response type.
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException
                                      or System.Text.Json.JsonException or NotSupportedException)
        {
            System.Diagnostics.Debug.WriteLine($"Forecast request failed: {ex.Message}");
            return null; // the caller falls back to cached data and shows a message
        }
    }
}

