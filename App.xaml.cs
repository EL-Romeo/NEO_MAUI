namespace NEO_MAUI;

public partial class App : Application
{
    // Menggunakan dependency injection untuk mendapatkan AppShell
    public App(AppShell appShell)
    {
        InitializeComponent();

        MainPage = appShell;
    }
}
