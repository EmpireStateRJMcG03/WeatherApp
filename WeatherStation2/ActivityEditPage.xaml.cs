// ActivityEditPage code-behind: the C# half of the add/edit activity form.
// The layout is in ActivityEditPage.xaml. This page binds to ActivityEditViewModel.
// Navigation here comes from the Planner page using Shell, for example
// "ActivityEdit" to add a new activity or "ActivityEdit?id=..." to edit one.
using WeatherStation2.ViewModels;

namespace WeatherStation2.Views;

// "partial" means this class is joined with the code MAUI generates from the .xaml file.
public partial class ActivityEditPage : ContentPage
{
    // Keeps a reference to the view model so other methods in this page can call it.
    private readonly ActivityEditViewModel _vm;

    // The view model is passed in by dependency injection (MAUI creates it and hands it to us).
    public ActivityEditPage(ActivityEditViewModel vm)
    {
        // Builds the controls described in the .xaml file. It must be called first.
        InitializeComponent();
        // Sets the BindingContext, which is the object all the {Binding ...} expressions in the XAML read from.
        // The same view model is also saved in _vm for later use.
        BindingContext = _vm = vm;
    }

    // Runs every time the page is about to be shown on screen.
    // "async void" is used because this is an event style method that is overriding a void method.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Asks the view model to load what it needs (settings and the activity being edited).
        // "await" waits for that to finish without freezing the screen.
        await _vm.InitAsync();
    }
}
