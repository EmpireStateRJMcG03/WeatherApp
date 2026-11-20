namespace WeatherStation1.Models;

public class AppSettings
{
    public string Theme { get; set; } = "System";             // System, Light, Dark
    public string TemperatureUnit { get; set; } = "Celsius";  // Celsius, Fahrenheit
    public string WindUnit { get; set; } = "km/h";            // km/h, mph, m/s