using WeatherStation2.ViewModels;

namespace WeatherStation2.Views;

public partial class PlannerPage : ContentPage
{
    private readonly PlannerViewModel _vm;

    public PlannerPage(PlannerViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    // Reloads every time, so a saved or deleted activity shows up when you come back.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
