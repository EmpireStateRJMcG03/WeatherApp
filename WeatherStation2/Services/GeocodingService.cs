using System.Net.Http.Json;
using WeatherStation2.Models;

namespace WeatherStation2.Services;

// Turns a typed place name into coordinates using Open-Meteo's free geocoding endpoint (no API key).
// Geocoding means converting a name like "Paris" into latitude and longitude.
// The search page uses this class, and LocationStore saves the place the user picks.
public class GeocodingService
{
    // One shared HttpClient for the whole app (static), reused for every request.
    // The 10 second timeout stops the app waiting forever on a bad connection.
    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(10) };

    // Searches for places matching the text the user typed.
    // Returns null if the request failed, or an empty list if nothing matched.
    // The CancellationToken lets the caller cancel the search, for example if the user types something new.
    public async Task<List<GeocodingResult>?> SearchAsync(string query, CancellationToken ct = default)
    {
        // EscapeDataString makes the typed text safe inside a web address (spaces and symbols are encoded).
        // We ask for up to 10 results, in English, as JSON.
        var url = "https://geocoding-api.open-meteo.com/v1/search" +
                  $"?name={Uri.EscapeDataString(query)}&count=10&language=en&format=json";
        try
        {
            // await waits for the download without freezing the screen.
            // The result is converted from JSON into a GeocodingResponse object.
            var response = await _http.GetFromJsonAsync<GeocodingResponse>(url, ct);
            // If the response or its Results is missing (null), we return an empty list instead of null.
            // That keeps "no matches" different from "the request failed".
            return response?.Results ?? new List<GeocodingResult>();
        }
        // Expected failures: network or HTTP error, timeout or cancel (both appear as TaskCanceledException),
        // JSON that does not match our model, or an unsupported response type.
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException
                                      or System.Text.Json.JsonException or NotSupportedException)
        {
            System.Diagnostics.Debug.WriteLine($"Geocoding failed: {ex.Message}");
            return null;
        }
    }
}
