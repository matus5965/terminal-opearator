namespace InformationSystemHZS.Models.HelperModels;

public sealed class RecordedIncidentDto
{
    public required string Type { get; set; }
    public required PositionDto Location { get; set; }
    public required string Description { get; set; }
    public required string IncidentStartTIme { get; set; }
    public required string AssignedStation { get; set; }
    public required string AssignedUnit { get; set; }
}