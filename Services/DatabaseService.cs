namespace NEO_MAUI.Services;

using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using NEO_MAUI.Models;
using NEO_MAUI.Data;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DatabaseService
{
    private readonly DatabaseContext _context;

    public DatabaseService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<Driver>> GetDriversAsync()
    {
        return await _context.Drivers.OrderBy(d => d.Id).ToListAsync();
    }

    public async Task<List<FuelData>> GetFuelDataAsync(int driverId, DateTime startDate, DateTime endDate)
    {
        return await _context.FuelData
            .Where(fd => fd.DriverId == driverId && fd.Date.Date >= startDate.Date && fd.Date.Date <= endDate.Date)
            .OrderByDescending(fd => fd.Date)
            .ToListAsync();
    }

    public async Task<StatsResult> GetStatsAsync(int driverId, DateTime startDate, DateTime endDate)
    {
        var data = await GetFuelDataAsync(driverId, startDate, endDate);
        if (!data.Any())
        {
            return new StatsResult();
        }

        var totalFuelCost = data.Sum(d => d.FuelCost);
        var totalServiceCost = data.Sum(d => d.ServiceCost);
        var kmData = data.Where(d => d.Kilometer > 0).ToList();
        var totalDistance = kmData.Any() ? kmData.Max(d => d.Kilometer) - kmData.Min(d => d.Kilometer) : 0;
        var avgConsumption = totalDistance > 0 ? totalFuelCost / totalDistance : 0;

        return new StatsResult
        {
            TotalCost = totalFuelCost + totalServiceCost,
            TotalServiceCost = totalServiceCost,
            TotalDistance = totalDistance,
            AvgConsumption = avgConsumption,
            TotalLiters = 0
        };
    }

    public async Task<Stream> ExportDataToExcelStreamAsync(int driverId, DateTime startDate, DateTime endDate)
    {
        var data = await GetFuelDataAsync(driverId, startDate, endDate);
        if (!data.Any())
        {
            throw new InvalidOperationException("Tidak ada data untuk diekspor pada rentang tanggal yang dipilih.");
        }

        var dailyTarget = 175.0;
        var transformedData = data.Select(d => new
        {
            Tanggal = d.Date.ToString("dd-MM-yyyy"),
            Plat_Nomor = d.Plate,
            Kilometer = d.Kilometer,
            Biaya_BBM = d.FuelCost,
            Jenis_Servis = d.ServiceType,
            Biaya_Servis = d.ServiceCost,
            Granit_dus = d.Granit,
            Keramik_dus = d.Keramik,
            Point = Math.Round(((d.Granit * 1.5) + d.Keramik) / (dailyTarget > 0 ? dailyTarget : 1), 2)
        }).ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Data Kendaraan");
        worksheet.Cell(1, 1).InsertTable(transformedData);
        worksheet.Columns().AdjustToContents();

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return stream;
    }
}

public class StatsResult
{
    public double TotalCost { get; set; }
    public double TotalServiceCost { get; set; }
    public double TotalDistance { get; set; }
    public double AvgConsumption { get; set; }
    public double TotalLiters { get; set; }
}
