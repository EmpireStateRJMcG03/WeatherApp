namespace WeatherStation2.Services;

// Remembers the last few locations the user looked at (newest first).
// It is saved in recent.json so the "recent" list is still there after the app restarts.
// Like the other list services, it stores only location names and uses StorageService for file access.
public class RecentService
{
    // The name of the JSON file inside the app's private data folder.
    private const string FileName = "recent.json";

    // The most names we keep. Older ones fall off the end of the list.
    private const int MaxItems = 5;

    // The helper that reads and writes files.
    private readonly StorageService _storage;

    // Recent location names, newest first, held in memory.
    private List<string> _names = new();

    // Remembers the loading job so the file is only read once. Null means loading has not started yet.
    private Task? _loadTask;

    // Read-only view of the recent names for pages to display.
    public IReadOnlyList<string> Names => _names;

    // Dependency injection: MAUI passes in the StorageService when it creates this class.
    public RecentService(StorageService storage) => _storage = storage;

    // Loads the list from disk the first time only. Later calls reuse the same task (the ??= operator).
    public Task LoadAsync() => _loadTask ??= LoadCoreAsync();

    // Reads the file without freezing the screen (async/await).
    private async Task LoadCoreAsync()
    {
        // A missing or damaged file gives back the empty list passed in as the fallback.
        _names = await _storage.LoadAsync(FileName, new List<string>());
        // Throw away blank entries in case the file was edited or damaged.
        _names.RemoveAll(string.IsNullOrWhiteSpace);
    }

    // Records that the user just viewed this location.
    public async Task AddAsync(string name)
    {
        // Remove any older copy first so the same place never appears twice.
        _names.Remove(name);
        // Put it at the front, because the list is newest first.
        _names.Insert(0, name);
        // If the list is now too long, cut off the oldest items at the end.
        if (_names.Count > MaxItems) _names.RemoveRange(MaxItems, _names.Count - MaxItems);
        await _storage.SaveAsync(FileName, _names);
    }
}
