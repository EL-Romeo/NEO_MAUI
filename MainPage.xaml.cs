namespace NEO_MAUI;

using NEO_MAUI.ViewModels;

// Ini adalah definisi kelas untuk halaman utama kita.
// Kata kunci 'partial' menandakan bahwa kelas ini terhubung dengan file XAML-nya.
public partial class MainPage : ContentPage
{
    // Constructor untuk MainPage.
    // Secara otomatis menerima instance MainPageViewModel dari Dependency Injection.
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();

        // Baris ini menyambungkan semua elemen UI di XAML ({Binding ...}) 
        // ke properti dan perintah di ViewModel.
        BindingContext = viewModel;
    }

    // Metode ini dijalankan setiap kali halaman muncul di layar.
    // Kita gunakan untuk memuat data awal jika belum ada.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MainPageViewModel vm)
        {
            // Hanya muat data jika koleksi driver masih kosong.
            if (vm.Drivers.Count == 0)
            {
                await vm.LoadInitialDataCommand.ExecuteAsync(null);
            }
        }
    }
}
