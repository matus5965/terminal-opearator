namespace InformationSystemHZS.Models.HelperModels;

public sealed class VehicleDto
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public double FuelConsumption { get; set; }
    public int Speed { get; set; }
    public int Capacity { get; set; }
}