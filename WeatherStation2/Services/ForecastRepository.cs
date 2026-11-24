using WeatherStation2.Models;
using Location = WeatherStation2.Models.Location;

namespace WeatherStation2.Services;

// What GetAsync returns: the forecast (null if there is none at all), when it was downloaded, and a stale flag.
// IsStale is true when the data came from the cache because the network request failed.
// A record is a short way to write a small read-only data class.
public record ForecastResult(ForecastResponse? Forecast, DateTime RetrievedUtc, bool IsStale);

// The one place that decides "use the cache or call the API". Every page asks this class for forecasts.
// It uses WeatherService to download and StorageService to keep the downloaded forecasts in a file.
// The cache is a dictionary of saved forecasts, with the location name as the key.
// Each saved forecast has one of three states:
//   fresh   - saved less than 30 minutes ago, so it is used with no network request.
//   stale   - older than that, so we try to refresh it, and use it only if the refresh fails.
//   missing - never saved, so the network is the only source.
public class ForecastRepository
{
    // The name of the JSON file that holds the cache.
    private const string CacheFile = "forecasts.json";

    // How old a saved forecast can be before we ask the API again.
    private static readonly TimeSpan MaxAge = TimeSpan.FromMinutes(30);

    // Both helpers are given to us by dependency injection (see the constructor).
    private readonly WeatherService _weather;
    private readonly StorageService _storage;

    // A SemaphoreSlim is a lock. Starting count 1 and maximum 1 means only one caller can be inside at a time.
    private readonly SemaphoreSlim _lock = new(1, 1);   // one lookup at a time, so the cache file is never written twice at once

    // The cache in memory, keyed by location name. It is null until the file is loaded the first time.
    private Dictionary<string, CachedForecast>? _cache;

    // Dependency injection: MAUI creates the WeatherService and StorageService and passes them in.
    public ForecastRepository(WeatherService weather, StorageService storage)
    {
        _weather = weather;
        _storage = storage;
    }

    // Gets the forecast for a location, from the cache or from the internet.
    // force = true skips the "fresh cache" shortcut, which is what the Refresh action on the Forecast page uses.
    // The result always comes back (it never throws for network problems). Check Forecast for null and IsStale.
    public async Task<ForecastResult> GetAsync(Location loc, bool force = false)
    {
        // Wait for our turn. If another call is running, this one pauses here (without freezing the screen).
        await _lock.WaitAsync();
        try
        {
            // First call only: load the cache file. ??= means "only if _cache is still null".
            _cache ??= await _storage.LoadAsync(CacheFile, new Dictionary<string, CachedForecast>());

            // Look for this location's saved entry. cached stays null when there is none.
            _cache.TryGetValue(loc.Name, out var cached);
            // True only if there is an entry that really contains a forecast.
            var hasCache = cached?.Forecast != null;

            // 1. Fresh enough: no network request at all.
            // The ! tells the compiler cached is not null here, because hasCache is true.
            // UtcNow is used so time zones and daylight saving cannot upset the age sum.
            if (hasCache && !force && DateTime.UtcNow - cached!.RetrievedAtUtc < MaxAge)
                return new ForecastResult(cached.Forecast, cached.RetrievedAtUtc, false);

            // 2. Ask the API.
            var fresh = await _weather.GetForecastAsync(loc);
            // Count the download as good only if it is not null and has some current or daily data in it.
            if (fresh != null && (fresh.Current != null || fresh.Daily != null))
            {
                // Store it with the current time, then save the whole cache to disk.
                var entry = new CachedForecast { Forecast = fresh, RetrievedAtUtc = DateTime.UtcNow };
                _cache[loc.Name] = entry;
                await _storage.SaveAsync(CacheFile, _cache);
                return new ForecastResult(fresh, entry.RetrievedAtUtc, false);
            }

            // 3. The request failed: fall back to older data if we have any.
            // IsStale is true so the page can tell the user this data may be out of date.
            if (hasCache) return new ForecastResult(cached!.Forecast, cached.RetrievedAtUtc, true);

            // Nothing from the network and nothing saved: return an empty result. default is the "no date" value.
            return new ForecastResult(null, default, true);
        }
        finally
        {
            // finally always runs, even after a return or an error, so the lock is always released.
            // Without this, one failure could block every later call forever.
            _lock.Release();
        }
    }
}
