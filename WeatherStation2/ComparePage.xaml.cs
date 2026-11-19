using WeatherStation2.ViewModels;

namespace WeatherStation2.Views;

public partial class ComparePage : ContentPage
{
    private readonly CompareViewModel _vm;

    public ComparePage(CompareViewModel vm)
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
