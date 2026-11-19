using WeatherStation1.ViewModels;

namespace WeatherStation1.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _vm;

    public SettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitAsync();
    }

    private async void OnCreditTapped(object? sender, TappedEventArgs e)
    {
        try { await Launcher.OpenAsync("https://open-meteo.com/"); }
        catch (Exception) { /* no browser available: ignore */ }
    }
}
