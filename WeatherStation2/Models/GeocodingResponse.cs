// GeocodingResponse.cs
// Describes the JSON from the Open-Meteo geocoding (place search) API.
// Geocoding means turning a place name such as "Cork" into latitude and longitude.
// GeocodingService uses these classes when the user searches for a new location.
using System.Text.Json.Serialization;

namespace WeatherStation2.Models;

// The whole reply: a list of places that matched the search text.
public class GeocodingResponse
{
    // If nothing matches, the API leaves "results" out entirely, so it must be nullable.
    // Null means no matches. Otherwise it is the list of matches.
    [JsonPropertyName("results")] public List<GeocodingResult>? Results { get; set; }
}

// One matching place.
public class GeocodingResult
{
    // The place name, for example "Galway".
    [JsonPropertyName("name")] public string? Name { get; set; }
    // Degrees north (positive) or south (negative) of the equator.
    [JsonPropertyName("latitude")] public double Latitude { get; set; }
    // Degrees east (positive) or west (negative) of the prime meridian.
    [JsonPropertyName("longitude")] public double Longitude { get; set; }
    // Country name. Can be missing, so it is nullable.
    [JsonPropertyName("country")] public string? Country { get; set; }
    // First-level administrative area, such as a county or state. Can be missing.
    [JsonPropertyName("admin1")] public string? Admin1 { get; set; }   // region or county
}
