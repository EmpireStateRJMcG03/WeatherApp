namespace WeatherStation2.Services;

// Keeps the list of favourite locations and saves it in favourites.json.
// Only the location NAMES are stored, because names are unique and act as the key everywhere in the app.
// Pages use this class to show the star state and to add or remove favourites.
// The real file work is done by StorageService, which is injected into the constructor.
public class FavouritesService
{
    // The name of the JSON file inside the app's private data folder.
    private const string FileName = "favourites.json";

    // The helper that reads and writes files.
    private readonly StorageService _storage;

    // Favourite location names, held in memory.
    private List<string> _names = new();

    // Remembers the loading job so the file is only read once. Null means loading has not started yet.
    private Task? _loadTask;

    // Read-only view of the favourite names for pages to display.
    public IReadOnlyList<string> Names => _names;

    // Dependency injection: MAUI passes in the StorageService when it creates this class.
    public FavouritesService(StorageService storage) => _storage = storage;

    // Loads favourites from disk the first time only. Later calls reuse the same task (the ??= operator).
    public Task LoadAsync() => _loadTask ??= LoadCoreAsync();

    // Reads the file without freezing the screen (async/await).
    private async Task LoadCoreAsync()
    {
        // A missing or damaged file gives back the empty list passed in as the fallback.
        _names = await _storage.LoadAsync(FileName, new List<string>());
        _names.RemoveAll(string.IsNullOrWhiteSpace);   // ignore junk in a damaged file
    }

    // True if this location name is already a favourite.
    public bool IsFavourite(string name) => _names.Contains(name);

    // Flips the favourite state: removes the name if it is there, otherwise adds it. Then saves.
    public async Task ToggleAsync(string name)
    {
        // List.Remove returns false when the name was not in the list. In that case we add it instead.
        if (!_names.Remove(name)) _names.Add(name);
        await _storage.SaveAsync(FileName, _names);
    }
}

