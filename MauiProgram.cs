namespace NEO_MAUI;

// Mengimpor namespace yang diperlukan
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using NEO_MAUI.Data;
using NEO_MAUI.Services;
using NEO_MAUI.ViewModels;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        //==================================================
        // PENDAFTARAN SEMUA LAYANAN (DEPENDENCY INJECTION)
        //==================================================
        builder.Services.AddDbContext<DatabaseContext>();
        builder.Services.AddTransient<DatabaseService>();
        builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<AppShell>();

        //==================================================
        // INISIALISASI DATABASE SAAT STARTUP
        //==================================================
        var serviceProvider = builder.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<DatabaseContext>();
        dbContext?.InitializeDatabase();

        return builder.Build();
    }
}
