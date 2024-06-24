using InformationSystemHZS.Models.Interfaces;

namespace InformationSystemHZS.Models.HelperModels;

public sealed class StationDto : IBaseModel
{
    public required string Callsign { get; set; }
    public required PositionDto PositionDto { get; set; }
    public required string Name { get; set; }
    public required List<UnitDto> Units { get; set; }
}