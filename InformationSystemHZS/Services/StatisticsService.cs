using InformationSystemHZS.Models;

namespace InformationSystemHZS.Services;

// IMPORTANT NOTE: For this part of code use only LINQ.
// NOTE: You can change the signature of functions to take a single parameter, i.e. something like List<Station>.

public static class StatisticsService
{
    /// <summary>
    /// Returns the total number of all 'FIRE_ENGINE' across all stations.
    /// </summary>
    public static int GetTotalFireEnginesCount(List<Station> stations)
    {
        return stations
            .SelectMany(s => s.GetAllUnits())
            .Where(u => u.UsedVehicle.Type.Equals(VehicleType.FIRE_ENGINE))
            .Count();
    }

    /// <summary>
    /// Returns the name of the station closest to the hospital at coordinates (45, 60).
    /// </summary>
    public static string GetClosestToHospital(List<Station> stations)
    {
        return stations
            .OrderBy(s => DistanceService.CalculateDistance(s.X, s.Y, 45, 60))
            .Select(s => s.Name)
            .First();
    }
    
    /// <summary>
    /// Returns the callsign of the unit with the fastest vehicle. If no decision can be made, an error is printed.
    /// </summary>
    public static string GetFastestVehicleUnit(List<Station> stations) 
    {
        var fastestUnits = stations
            .SelectMany(s => s.GetAllUnits())
            .GroupBy(u => u.UsedVehicle.Speed)
            .OrderByDescending(g => g.Key)
            .First();

        return (fastestUnits.Count() == 1) ? fastestUnits.First().Callsign : "ERROR";
    }

    /// <summary>
    /// Returns the callsign of the station that has the most firefighters under it.
    /// </summary>
    public static string GetStationWithMostPersonel(List<Station> stations)
    {
        return stations
            .OrderByDescending(s => s.GetAllUnits()
            .Sum(u => u.GetMembersCount()))
            .Select(u => u.Callsign)
            .First();
    }

    /// <summary>
    /// Returns a list of all vehicle names sorted by fuel consumption (first with the lowest, last with the highest).
    /// Duplicate names must not appear in the list.
    /// </summary>
    public static List<string> GetVehiclesByFuelConsumption(List<Station> stations)
    {
        return stations
            .SelectMany(s => s.GetAllUnits())
            .Select(u => u.UsedVehicle)
            .OrderBy(v => v.FuelConsumption)
            .Select(v => v.Name)
            .Distinct()
            .ToList();
    }

    /// <summary>
    /// Returns the callsign of the unit that has historically resolved the highest number of events.
    /// </summary>
    public static string GetMostBusyUnit(List<Station> stations)
    {
        return stations
            .SelectMany(s => s.GetAllUnits())
            .OrderByDescending(u => u.ResolvedIncidents)
            .Select(u => u.Callsign)
            .First();
    }

    /// <summary>
    /// Returns the callsign of the unit that has consumed the most fuel with its vehicle in the sum of all its historical events.
    /// </summary>
    public static string MostFuelConsumedUnit(List<Station> stations)
    {
        return stations
            .SelectMany(s => s.GetAllUnits())
            .OrderByDescending(u => u.ConsumedFuel)
            .Select(u => u.Callsign)
            .First();
    }
}