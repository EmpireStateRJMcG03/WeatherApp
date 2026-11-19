using WeatherStation2.Helpers;
using WeatherStation2.ViewModels;

namespace WeatherStation2.Views;

public partial class ForecastPage : ContentPage
{
    private readonly ForecastViewModel _vm;

    public ForecastPage(ForecastViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Fade the page in each time it appears, while the data loads.
        Root.Opacity = 0;
        _ = Root.FadeToAsync(1, 400);
        await _vm.InitAsync();
    }

    private async void OnChangeLocation(object? sender, EventArgs e)
    {
        var names = LocationData.All.Select(l => l.Name).ToArray();
        var pick = await DisplayActionSheetAsync("Choose location", "Cancel", null, names);
        if (pick is null or "Cancel") return;
        await _vm.LoadAsync(LocationData.Find(pick));
    }

    // A small "pop" so the favourite star feels responsive.
    private async void OnStarClicked(object? sender, EventArgs e)
    {
        await StarButton.ScaleToAsync(1.4, 100);
        await StarButton.ScaleToAsync(1.0, 100);
    }

    private async void OnCreditTapped(object? sender, TappedEventArgs e)
    {
        try { await Launcher.OpenAsync("https://open-meteo.com/"); }
        catch (Exception) { /* no browser available: ignore */ }
    }
}
