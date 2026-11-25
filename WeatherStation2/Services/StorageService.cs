using System.Text.Json;

namespace WeatherStation2.Services;

// Saves and loads any object as JSON in the app's private data folder.
// Every other service (favourites, recent, settings, activities, forecast cache) uses this class
// so the file handling code lives in one place only.
// JSON is a plain text format for data. System.Text.Json turns C# objects into JSON text and back.
public class StorageService
{
    // Builds the full path to a file in the app's private data folder, which the OS provides per app.
    // Static because it needs no object data. It is private because only this class needs it.
    private static string PathFor(string fileName) =>
        Path.Combine(FileSystem.AppDataDirectory, fileName);

    // Saves any object to a file as JSON. The generic type T means "any type of data".
    // If saving fails the error is logged and ignored, so the app keeps running (the data just isn't saved).
    public async Task SaveAsync<T>(string fileName, T data)
    {
        try
        {
            var path = PathFor(fileName);
            // The temporary file sits next to the real one and has ".tmp" added to its name.
            var temp = path + ".tmp";

            // Write to a temporary file first so a crash mid-save can't leave a half-written file.
            // Serialize turns the object into JSON text (default options are used).
            await File.WriteAllTextAsync(temp, JsonSerializer.Serialize(data));
            // Only when the temp file is complete do we swap it in. overwrite: true replaces the old file.
            File.Move(temp, path, overwrite: true);
        }
        // Only catch the problems we expect: disk or file errors, no permission, or an unsupported path.
        // Other kinds of error are real bugs, so we let them show up instead of hiding them.
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            System.Diagnostics.Debug.WriteLine($"Save failed ({fileName}): {ex.Message}");
        }
    }

    // Loads an object from a JSON file. If there is no file, or it cannot be read, the fallback value is returned.
    // That means callers never have to deal with errors and always get something usable.
    public async Task<T> LoadAsync<T>(string fileName, T fallback)
    {
        try
        {
            var path = PathFor(fileName);
            if (!File.Exists(path)) return fallback;          // first run

            var json = await File.ReadAllTextAsync(path);
            // Deserialize turns the JSON text back into an object. It can return null (for example if the file says null),
            // so ?? fallback gives us the fallback in that case too.
            return JsonSerializer.Deserialize<T>(json) ?? fallback;
        }
        // Expected problems: file cannot be read, no permission, the text is not valid JSON, or the type is unsupported.
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
                                      or JsonException or NotSupportedException)
        {
            System.Diagnostics.Debug.WriteLine($"Load failed ({fileName}): {ex.Message}");
            return fallback;                                   // damaged file
        }
    }
}

