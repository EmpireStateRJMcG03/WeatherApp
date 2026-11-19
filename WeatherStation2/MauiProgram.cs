using Microsoft.Extensions.Logging;
using WeatherStation2.Services;
using WeatherStation2.ViewModels;
using WeatherStation2.Views;
using WeatherStation2;

namespace WeatherStation2;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Services: one shared instance each for the whole app.
        builder.Services.AddSingleton<StorageService>();
        builder.Services.AddSingleton<SettingsService>();
        builder.Services.AddSingleton<FavouritesService>();
        builder.Services.AddSingleton<RecentService>();
        builder.Services.AddSingleton<ActivityService>();
        builder.Services.AddSingleton<WeatherService>();
        builder.Services.AddSingleton<GeocodingService>();
        builder.Services.AddSingleton<ForecastRepository>();
        builder.Services.AddSingleton<LocationStore>();
        builder.Services.AddSingleton<ImageService>();
        builder.Services.AddSingleton<UnitFormatter>();

        // View models and pages: a new one each time they are needed.
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<ForecastViewModel>();
        builder.Services.AddTransient<CompareViewModel>();
        builder.Services.AddTransient<PlannerViewModel>();
        builder.Services.AddTransient<ActivityEditViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ForecastPage>();
        builder.Services.AddTransient<ComparePage>();
        builder.Services.AddTransient<PlannerPage>();
        builder.Services.AddTransient<ActivityEditPage>();
        builder.Services.AddTransient<SettingsPage>();

        return builder.Build();
    }
}
