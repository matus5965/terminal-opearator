using InformationSystemHZS.Models.Interfaces;

namespace InformationSystemHZS.Models.HelperModels;

public sealed class MemberDto : IBaseModel
{
    public required string Callsign { get; set; }
    public required string UnitCallsign { get; set; }
    public required string Name { get; set; }
    public required string Rank { get; set; }
}