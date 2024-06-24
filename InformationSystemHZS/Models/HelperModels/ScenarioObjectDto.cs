namespace InformationSystemHZS.Models.HelperModels;

public sealed class ScenarioObjectDto
{
    public required List<StationDto> Stations { get; set; }
    public required List<RecordedIncidentDto> IncidentsHistory { get; set; }
}