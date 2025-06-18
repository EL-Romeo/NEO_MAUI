namespace NEO_MAUI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Storage;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Linq;
using NEO_MAUI.Models;
using NEO_MAUI.Services;

public partial class MainPageViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;
    private readonly IFileSaver _fileSaver;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<Driver> _drivers;

    [ObservableProperty]
    private ObservableCollection<FuelData> _fuelHistory;

    [ObservableProperty]
    private ObservableCollection<FuelData> _serviceHistory;

    [ObservableProperty]
    private ObservableCollection<FuelData> _loadHistory;

    [ObservableProperty]
    private StatsResult _stats;

    [ObservableProperty]
    private Driver? _selectedDriver;

    [ObservableProperty]
    private DateTime _startDate;

    [ObservableProperty]
    private DateTime _endDate;

    public MainPageViewModel(DatabaseService databaseService, IFileSaver fileSaver)
    {
        _dbService = databaseService;
        _fileSaver = fileSaver;
        _drivers = new ObservableCollection<Driver>();
        _fuelHistory = new ObservableCollection<FuelData>();
        _serviceHistory = new ObservableCollection<FuelData>();
        _loadHistory = new ObservableCollection<FuelData>();
        _stats = new StatsResult();
        var today = DateTime.Now;
        _startDate = new DateTime(today.Year, today.Month, 1);
        _endDate = today;
    }

    [RelayCommand]
    private async Task LoadInitialDataAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var driversList = await _dbService.GetDriversAsync();
            if (driversList is not null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Drivers.Clear();
                    foreach (var driver in driversList)
                    {
                        Drivers.Add(driver);
                    }
                    if (Drivers.Any())
                    {
                        SelectedDriver = Drivers.First();
                    }
                });
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Gagal memuat data driver: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ExportDataAsync()
    {
        if (IsBusy || SelectedDriver == null) return;
        IsBusy = true;
        try
        {
            using var excelStream = await _dbService.ExportDataToExcelStreamAsync(SelectedDriver.Id, StartDate, EndDate);
            var fileName = $"Export_{SelectedDriver.Plate.Replace(" ", "")}_{DateTime.Now:yyyyMMdd}.xlsx";
            var fileSaverResult = await _fileSaver.SaveAsync(fileName, excelStream, CancellationToken.None);
            if (fileSaverResult.IsSuccessful)
            {
                await Shell.Current.DisplayAlert("Sukses", $"File berhasil disimpan di: {fileSaverResult.FilePath}", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Gagal", "Tidak dapat menyimpan file.", "OK");
            }
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlert("Info", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Terjadi kesalahan saat ekspor: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedDriverChanged(Driver? value)
    {
        if (value != null)
        {
            Task.Run(RefreshAllDataAsync);
        }
    }

    partial void OnStartDateChanged(DateTime value)
    {
        Task.Run(RefreshAllDataAsync);
    }

    partial void OnEndDateChanged(DateTime value)
    {
        Task.Run(RefreshAllDataAsync);
    }

    private async Task RefreshAllDataAsync()
    {
        if (SelectedDriver == null) return;

        IsBusy = true;
        try
        {
            var statsTask = _dbService.GetStatsAsync(SelectedDriver.Id, StartDate, EndDate);
            var historyTask = _dbService.GetFuelDataAsync(SelectedDriver.Id, StartDate, EndDate);
            await Task.WhenAll(statsTask, historyTask);

            var history = await historyTask;
            var stats = await statsTask;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Stats = stats;
                FuelHistory.Clear();
                ServiceHistory.Clear();
                LoadHistory.Clear();

                if (history is not null)
                {
                    foreach (var record in history)
                    {
                        if (record.FuelCost > 0 || record.Kilometer > 0)
                            FuelHistory.Add(record);
                        if (record.ServiceCost > 0 || !string.IsNullOrWhiteSpace(record.ServiceType))
                            ServiceHistory.Add(record);
                        if (record.Granit > 0 || record.Keramik > 0)
                            LoadHistory.Add(record);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Gagal memuat data riwayat: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
