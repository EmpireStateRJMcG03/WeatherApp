// HomePage code-behind: the C# half of the Locations page.
// The layout is in HomePage.xaml. This page binds to HomeViewModel.
// It also handles searching for a new city, tapping a saved location and tapping the avatar.
using ObjCRuntime;
using UIKit;
using WeatherStation2.Models;
using WeatherStation2.Services;
using WeatherStation2.ViewModels;

namespace WeatherStation2.Views;

public partial class HomePage : ContentPage
{
    // Fields that keep the objects given to the constructor, so the methods below can use them.
    // _vm is the view model, _geo looks up cities by name and _store saves locations.
    private readonly HomeViewModel _vm;
    private readonly GeocodingService _geo;
    private readonly LocationStore _store;

    // The three objects are supplied by dependency injection (MAUI creates them and passes them in).
    public HomePage(HomeViewModel vm, GeocodingService geo, LocationStore store)
    {
        // Creates the controls defined in HomePage.xaml.
        InitializeComponent();
        // Makes the view model the source for every {Binding ...} in the XAML, and also stores it in _vm.
        BindingContext = _vm = vm;
        _geo = geo;
        _store = store;
    }

    // Runs each time the page is about to appear, so the list is up to date when you come back to this tab.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Loads the saved locations and rebuilds the groups (see HomeViewModel.LoadAsync).
        await _vm.LoadAsync();
    }

    // Search only runs when the user presses the search button, never on every keystroke.
    private async void OnSearch(object? sender, EventArgs e)
    {
        // Read the text typed in the search bar and remove spaces at both ends. The ? avoids an error if Text is null.
        var query = Search.Text?.Trim();
        // Do nothing if the box is empty.
        if (string.IsNullOrEmpty(query)) return;

        // Disable the search bar while working, so the user cannot start a second search.
        Search.IsEnabled = false;
        // try/finally makes sure the search bar is switched back on at the end, even after an early return or an error.
        try
        {
            // Ask the geocoding service (an online city search) for matching places. await waits without freezing the screen.
            var results = await _geo.SearchAsync(query);
            // null means the request itself failed, for example no internet.
            if (results == null)
            {
                await DisplayAlertAsync("Search failed",
                    "Couldn't reach the search service. Check your connection and try again.", "OK");
                return;
            }
            // An empty list means the request worked but no place matched.
            if (results.Count == 0)
            {
                await DisplayAlertAsync("No results", $"No places found for \"{query}\".", "OK");
                return;
            }

            // Number each label so two identical-looking places can't be mixed up.
            // Turn each result into a text label for the menu, for example "1. Dublin, Leinster, Ireland".
            var labels = results.Select((r, i) => $"{i + 1}. {Describe(r)}").ToArray();
            // Show a pop-up menu of the labels. Arguments: title, cancel button text, destroy button (none), then the choices.
            // It returns the text of the chosen label, or null if cancelled.
            var pick = await DisplayActionSheetAsync("Choose a place", "Cancel", null, labels);
            // Find the position of the chosen label in the list. -1 means nothing was chosen.
            var index = pick == null ? -1 : Array.IndexOf(labels, pick);
            if (index < 0) return;

            // Save the chosen place in the store so it appears in the list.
            var location = await _store.AddAsync(results[index]);
            // Clear the search box.
            Search.Text = "";
            // Navigate with Shell. // at the start of the route means go to a top level tab (ForecastPage).
            // ?name=... is a query string that passes the place name to the page.
            // Uri.EscapeDataString makes the name safe to put in a web style address (spaces and symbols are encoded).
            await Shell.Current.GoToAsync($"//ForecastPage?name={Uri.EscapeDataString(location.Name)}");
        }
        finally
        {
            // Always runs at the end: switch the search bar back on.
            Search.IsEnabled = true;
        }
    }

    // Builds a readable name from a search result, for example "Cork, Munster, Ireland".
    // Where() drops any parts that are empty and string.Join puts the rest together with commas.
    private static string Describe(GeocodingResult r) =>
        string.Join(", ", new[] { r.Name, r.Admin1, r.Country }.Where(s => !string.IsNullOrWhiteSpace(s)));

    // Runs when the user taps a location row in the list.
    private async void OnSelected(object? sender, SelectionChangedEventArgs e)
    {
        // Get the tapped item. If there is none (or it is not a LocationRow) stop.
        if (e.CurrentSelection.FirstOrDefault() is not LocationRow row) return;

        ((CollectionView)sender!).SelectedItem = null;   // so the same row can be tapped again
        // Go to the forecast tab and pass the location name in the query string.
        await Shell.Current.GoToAsync($"//ForecastPage?name={Uri.EscapeDataString(row.Name)}");
    }

    // Runs when the avatar is tapped. Goes to the Settings tab (top level route SettingsPage).
    private async void OnAvatarTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//SettingsPage");
}
