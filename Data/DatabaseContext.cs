namespace NEO_MAUI.Data;

using Microsoft.EntityFrameworkCore;
using NEO_MAUI.Models;
using System.IO;
using System;
using System.Linq;

public class DatabaseContext : DbContext
{
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<FuelData> FuelData { get; set; }

    private readonly string _dbPath;

    public DatabaseContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        _dbPath = Path.Join(path, "vehicle_data.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_dbPath}");

    public void InitializeDatabase()
    {
        try
        {
            Database.EnsureCreated();
            if (!Drivers.Any())
            {
                var initialDrivers = new[]
                {
                    new Driver { Name = "PRAPTO", Plate = "AE 9481 BD", FuelType = "Solar", Jenis = "L-300" },
                    new Driver { Name = "ARDA", Plate = "AE 9482 BD", FuelType = "Solar", Jenis = "L-300" },
                    new Driver { Name = "YAYAN", Plate = "AE 9650 BD", FuelType = "Solar", Jenis = "L-300" },
                    new Driver { Name = "DANU", Plate = "AE 8019 BD", FuelType = "Solar", Jenis = "L-300" },
                    new Driver { Name = "INDRA", Plate = "AE 8380 BF", FuelType = "Solar", Jenis = "TRAGA" },
                    new Driver { Name = "TOTOK", Plate = "AG 9874 VJ", FuelType = "Solar", Jenis = "TRAGA" },
                    new Driver { Name = "PARNO", Plate = "AE 9415 BE", FuelType = "Solar", Jenis = "TRAGA" },
                    new Driver { Name = "AMAT", Plate = "AE 8432 BB", FuelType = "Solar", Jenis = "ENGKEL" },
                    new Driver { Name = "NO KOCO", Plate = "AE 8039 BB", FuelType = "Solar", Jenis = "ENGKEL" },
                    new Driver { Name = "JOKO TMB", Plate = "AE 8065 BD", FuelType = "Solar", Jenis = "DOBEL" },
                    new Driver { Name = "GUDEL", Plate = "AE 8083 UC", FuelType = "Solar", Jenis = "DOBEL" },
                    new Driver { Name = "MARLAN", Plate = "AE 8068 BD", FuelType = "Solar", Jenis = "DOBEL" },
                    new Driver { Name = "UDIN", Plate = "AE 4003 BO", FuelType = "Pertalite", Jenis = "VIAR" },
                };
                Drivers.AddRange(initialDrivers);
                SaveChanges();
            }
        }
        catch (Exception ex)
        {
            // Tambahkan logging atau handling error di sini jika diperlukan
            Console.WriteLine($"Error initializing database: {ex.Message}");
        }
    }
}
