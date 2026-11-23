// SettingsPage code-behind: the C# half of the Settings page.
// The layout is in SettingsPage.xaml. This page binds to SettingsViewModel.
using WeatherStation2.ViewModels;

namespace WeatherStation2.Views;

public partial class SettingsPage : ContentPage
{
    // Reference to the view model, kept so OnAppearing can call it.
    private readonly SettingsViewModel _vm;

    // The view model is supplied by dependency injection.
    public SettingsPage(SettingsViewModel vm)
    {
        // Creates the controls defined in the .xaml file.
        InitializeComponent();
        // Makes the view model the source for every {Binding ...} in the XAML, and also stores it in _vm.
        BindingContext = _vm = vm;
    }

    // Runs each time the page is about to appear, so the settings shown are current.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Loads the saved settings into the view model (see SettingsViewModel.InitAsync).
        await _vm.InitAsync();
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
