using ObjCRuntime;
using WeatherStation2.Models;
using WeatherStation2.Services;
using WeatherStation2.ViewModels;

namespace WeatherStation2.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _vm;
    private readonly GeocodingService _geo;
    private readonly LocationStore _store;

    public HomePage(HomeViewModel vm, GeocodingService geo, LocationStore store)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _geo = geo;
        _store = store;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    // Search only runs when the user presses the search button, never on every keystroke.
    private async void OnSearch(object? sender, EventArgs e)
    {
        var query = Search.Text?.Trim();
        if (string.IsNullOrEmpty(query)) return;

        Search.IsEnabled = false;
        try
        {
            var results = await _geo.SearchAsync(query);
            if (results == null)
            {
                await DisplayAlertAsync("Search failed",
                    "Couldn't reach the search service. Check your connection and try again.", "OK");
                return;
            }
            if (results.Count == 0)
            {
                await DisplayAlertAsync("No results", $"No places found for \"{query}\".", "OK");
                return;
            }

            // Number each label so two identical-looking places can't be mixed up.
            var labels = results.Select((r, i) => $"{i + 1}. {Describe(r)}").ToArray();
            var pick = await DisplayActionSheetAsync("Choose a place", "Cancel", null, labels);
            var index = pick == null ? -1 : Array.IndexOf(labels, pick);
            if (index < 0) return;

            var location = await _store.AddAsync(results[index]);
            Search.Text = "";
            await Shell.Current.GoToAsync($"//ForecastPage?name={Uri.EscapeDataString(location.Name)}");
        }
        finally
        {
            Search.IsEnabled = true;
        }
    }

    private static string Describe(GeocodingResult r) =>
        string.Join(", ", new[] { r.Name, r.Admin1, r.Country }.Where(s => !string.IsNullOrWhiteSpace(s)));

    private async void OnSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not LocationRow row) return;

        ((CollectionView)sender!).SelectedItem = null;   // so the same row can be tapped again
        await Shell.Current.GoToAsync($"//ForecastPage?name={Uri.EscapeDataString(row.Name)}");
    }

    private async void OnAvatarTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//SettingsPage");
}
