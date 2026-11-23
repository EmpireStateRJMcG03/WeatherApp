// ComparePage code-behind: the C# half of the page that compares two places.
// The layout is in ComparePage.xaml. This page binds to CompareViewModel.
using WeatherStation2.ViewModels;

namespace WeatherStation2.Views;

public partial class ComparePage : ContentPage
{
    // Reference to the view model, kept so OnAppearing can call it.
    private readonly CompareViewModel _vm;

    // The view model is supplied by dependency injection.
    public ComparePage(CompareViewModel vm)
    {
        // Creates the controls defined in the .xaml file.
        InitializeComponent();
        // Makes the view model the source for every {Binding ...} in the XAML, and also stores it in _vm.
        BindingContext = _vm = vm;
    }

    // Runs each time the page is about to appear, so the data is refreshed when you come back to it.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Loads settings and favourites, then the weather (see CompareViewModel.InitAsync).
        await _vm.InitAsync();
    }

    // Runs when the "Weather data by Open-Meteo.com" label is tapped.
    private async void OnCreditTapped(object? sender, TappedEventArgs e)
    {
        // Launcher.OpenAsync opens the web address in the device's default browser.
        // try/catch is used so the app does not crash if the device cannot open a browser.
        try { await Launcher.OpenAsync("https://open-meteo.com/"); }
        catch (Exception) { /* no browser available: ignore */ }
    }
}
