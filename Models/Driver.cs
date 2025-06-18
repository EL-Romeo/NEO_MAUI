namespace NEO_MAUI.Models;

using System.ComponentModel.DataAnnotations;

public class Driver
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public string Jenis { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
}
