using WeatherStation2.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WeatherStation2.Services;

// Holds the user's settings (theme, temperature unit, wind unit and so on) and saves them in settings.json.
// The settings page changes the values on Current and then calls SaveAsync.
// UnitFormatter reads these settings to decide how to show temperatures and wind speeds.
// This class also applies the light/dark theme to the whole app.
public class SettingsService
{
    // The name of the JSON file inside the app's private data folder.
    private const string FileName = "settings.json";

    // The helper that reads and writes files.
    private readonly StorageService _storage;

    // Remembers the loading job so the file is only read once. Null means loading has not started yet.
    private Task? _loadTask;

    // The settings in use right now. Anyone can read or edit the values, but only this class can swap in a new object (private set).
    // It starts with default settings so the app works even before the file has loaded.
    public AppSettings Current { get; private set; } = new();

    // Dependency injection: MAUI passes in the StorageService when it creates this class.
    public SettingsService(StorageService storage) => _storage = storage;

    // Safe to call from many pages: the file is only read once.
    // The ??= operator starts the load only if it has not started yet, otherwise it reuses the same task.
    public Task LoadAsync() => _loadTask ??= LoadCoreAsync();

    // Reads the settings file, then applies the saved theme.
    private async Task LoadCoreAsync()
    {
        // If the file is missing or damaged we get a fresh default AppSettings back.
        Current = await _storage.LoadAsync(FileName, new AppSettings());
        ApplyTheme();
    }

    // Saves the current settings to disk, then applies the theme so a theme change shows immediately.
    public async Task SaveAsync()
    {
        await _storage.SaveAsync(FileName, Current);
        ApplyTheme();
    }

    // Tells MAUI which colour theme to use.
    private void ApplyTheme()
    {
        // Application.Current can be null very early in start-up, so do nothing in that case.
        if (Application.Current == null) return;
        // A switch expression: it picks a result based on the saved text.
        // The underscore is the "anything else" case, which means follow the phone's own light/dark setting.
        Application.Current.UserAppTheme = Current.Theme switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }
}
