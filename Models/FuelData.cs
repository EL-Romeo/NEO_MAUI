namespace NEO_MAUI.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class FuelData
{
    [Key]
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Plate { get; set; } = string.Empty;
    public double FuelCost { get; set; }
    public double Kilometer { get; set; }
    public string FuelType { get; set; } = string.Empty;
    public double Granit { get; set; }
    public double Keramik { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public double ServiceCost { get; set; }
    public int DriverId { get; set; }
    [ForeignKey("DriverId")]
    public Driver Driver { get; set; } = null!;
}
