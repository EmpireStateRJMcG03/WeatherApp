// ForecastPage code-behind: the C# half of the Forecast page.
// The layout is in ForecastPage.xaml. This page binds to ForecastViewModel.
using WeatherStation2.Helpers;
using WeatherStation2.ViewModels;

namespace WeatherStation2.Views;

public partial class ForecastPage : ContentPage
{
    // Reference to the view model, kept so the methods below can call it.
    private readonly ForecastViewModel _vm;

    // The view model is supplied by dependency injection.
    public ForecastPage(ForecastViewModel vm)
    {
        // Creates the controls defined in the .xaml file.
        InitializeComponent();
        // Makes the view model the source for every {Binding ...} in the XAML, and also stores it in _vm.
        BindingContext = _vm = vm;
    }

    // Runs each time the page is about to appear (for example when you switch to this tab).
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Fade the page in each time it appears, while the data loads.
        // Opacity 0 means fully see-through. Root is the main column named in the XAML.
        Root.Opacity = 0;
        // Fades to fully visible (1) over 400 milliseconds. The _ = means we do not wait for it, so loading starts at the same time.
        _ = Root.FadeToAsync(1, 400);
        // Loads the location (for example the one picked on the Locations tab) and then its weather. See ForecastViewModel.InitAsync.
        await _vm.InitAsync();
    }

    // Runs when "Change location" is tapped. Shows a menu of the built in locations and loads the chosen one.
    private async void OnChangeLocation(object? sender, EventArgs e)
    {
        // Make an array of just the location names, to use as menu choices.
        var names = LocationData.All.Select(l => l.Name).ToArray();
        // Show a pop-up menu. Returns the chosen text, or null if the user dismisses it.
        var pick = await DisplayActionSheetAsync("Choose location", "Cancel", null, names);
        // Do nothing if the menu was dismissed or Cancel was chosen.
        if (pick is null or "Cancel") return;
        // Find the location with that name and load its weather.
        await _vm.LoadAsync(LocationData.Find(pick));
    }

    // A small "pop" so the favourite star feels responsive.
    // Runs when the star button is clicked (as well as its Command in the view model).
    private async void OnStarClicked(object? sender, EventArgs e)
    {
        // Grow to 1.4 times the size in 100 milliseconds...
        await StarButton.ScaleToAsync(1.4, 100);
        // ...then shrink back to normal size. await makes the second animation start after the first ends.
        await StarButton.ScaleToAsync(1.0, 100);
    }

    // Runs when the "Weather data by Open-Meteo.com" label is tapped.
    private async void OnCreditTapped(object? sender, TappedEventArgs e)
    {
        // Launcher.OpenAsync opens the web address in the default browser.
        // try/catch stops the app crashing if the device cannot open a browser.
        try { await Launcher.OpenAsync("https://open-meteo.com/"); }
        catch (Exception) { /* no browser available: ignore */ }
    }
}
