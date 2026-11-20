namespace WeatherStation1.Models;

// A forecast plus the moment it was downloaded (the brief requires storing the retrieval time).
public class CachedForecast
{
    public ForecastResponse? Forecast { get; set; }
    public DateTime RetrievedAtUtc { get; set; }
}