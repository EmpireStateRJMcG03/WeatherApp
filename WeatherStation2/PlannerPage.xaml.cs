// PlannerPage code-behind: the C# half of the Planner page.
// The layout is in PlannerPage.xaml. This page binds to PlannerViewModel.
using WeatherStation2.ViewModels;

namespace WeatherStation2.Views;

public partial class PlannerPage : ContentPage
{
    // Reference to the view model, kept so OnAppearing can call it.
    private readonly PlannerViewModel _vm;

    // The view model is supplied by dependency injection.
    public PlannerPage(PlannerViewModel vm)
    {
        // Creates the controls defined in the .xaml file.
        InitializeComponent();
        // Makes the view model the source for every {Binding ...} in the XAML, and also stores it in _vm.
        BindingContext = _vm = vm;
    }

    // Reloads every time, so a saved or deleted activity shows up when you come back.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Loads the saved activities and their forecasts (see PlannerViewModel.LoadAsync).
        await _vm.LoadAsync();
    }
}
