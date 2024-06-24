using InformationSystemHZS.Models.Interfaces;

namespace InformationSystemHZS.Models.HelperModels;

public sealed class UnitDto : IBaseModel
{
    public required string Callsign { get; set; }
    public required string StationCallsign { get; set; }
    public required string State { get; set; }
    public required VehicleDto VehicleDto { get; set; }
    public required List<MemberDto> Members { get; set; }
}