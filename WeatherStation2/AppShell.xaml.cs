using WeatherStation2.Views;

namespace WeatherStation2;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // The add/edit page isn't a tab, so it needs a route to be navigated to.
        Routing.RegisterRoute("ActivityEdit", typeof(ActivityEditPage));
    }
}
