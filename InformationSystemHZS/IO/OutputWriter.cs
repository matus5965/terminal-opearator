using InformationSystemHZS.IO.Helpers.Interfaces;
using InformationSystemHZS.Models;
using InformationSystemHZS.Models.Interfaces;
using InformationSystemHZS.Services;

namespace InformationSystemHZS.IO;

public class OutputWriter
{
    private IConsoleManager _consoleManager;

    public OutputWriter(IConsoleManager consoleManager)
    {
        _consoleManager = consoleManager;
    }

    // General
    public void UserInputIndent()
    {
        Console.Write(">>> "); // We are not allowed to modify ConsoleManager, therefor not using _consoleManager.WriteLine(...)
    }

    // ERROR
    public void ImportExceptionOccured()
    {
        _consoleManager.WriteLine("[import]: Unexpected error happend during loading scenraio.");
    }

    public void ImportInvalidData(string errorMessage)
    {
        _consoleManager.WriteLine($"[import]: {errorMessage}");
    }

    public void InvalidArgument(string argumentName, string got, string expected)
    {
        _consoleManager.WriteLine($"[invalid]: Argument <{argumentName}> is invalid. Exptected {expected}. Got \"{got}\".");
    }

    public void InvalidArgumentCount(string command, int got, int expected)
    {
        _consoleManager.WriteLine($"[invalid]: For {command} expected {expected} arguments, got {got}.");
    }


    public void UnknownCommand()
    {
        _consoleManager.WriteLine($"[unknown]: Such command does not exist.");
    }

    public void CapacityError(string reason)
    {
        _consoleManager.WriteLine($"[capacity]: Not possible, because {reason}");
    }

    // List actions
    public void ListEntities<T>(List<T> entities)
    {
        foreach (T entity in entities)
        {
            string? textRepresentation;
            if (entity != null && (textRepresentation = entity.ToString()) != null)
            {
                _consoleManager.WriteLine(textRepresentation);
            }
        }

        // _consoleManager.WriteLine("");
    }

    // Assign actions
    public void EntityAssigned(string entityName, IModel? from, IModel? to, string assignGroup)
    {
        if (from == null && to != null)
        {
            _consoleManager.WriteLine($"[processed]: {entityName} was added to {assignGroup} {to.Callsign}.");
        }
        else if (from != null && to == null)
        {
            _consoleManager.WriteLine($"[processed]: {entityName} was removed from {assignGroup} {from.Callsign}");
        }
        else if (from != null && to != null)
        {
            _consoleManager.WriteLine($"[processed]: {entityName} was reassigned from {assignGroup} {from.Callsign} to {assignGroup} {to.Callsign}");
        }

    }

    public void IncidentAssigned(Unit unit)
    {
        string stationCallsign = "";
        if (unit.AssignedStation != null)
        {
            stationCallsign = $"{unit.AssignedStation.Callsign}:";
        }

        _consoleManager.WriteLine($"[processed]: Incident was assigned to unit {stationCallsign}{unit.Callsign}");
    }

    // Statistics
    public void Statistics(ScenarioStatistics statistics)
    {
        _consoleManager.WriteLine($"Total number of fire engines: {statistics.TotalFireEnginesCount}");
        _consoleManager.WriteLine($"The closest station to the hospital: {statistics.ClosestToHospital}");
        _consoleManager.WriteLine($"Fastest unit: {statistics.FastestVehicleUnit}");
        _consoleManager.WriteLine($"Station with the most firefighters: {statistics.StationWithMostPersonel}");
        _consoleManager.WriteLine($"Vehicle by fuel consumption: {string.Join(", ", statistics.VehiclesByFuelConsumption)}");
        _consoleManager.WriteLine($"Busiest unit: {statistics.MostBusyUnit}");
        _consoleManager.WriteLine($"Most fuel consumed: {statistics.MostFuelConsumedUnit}");
    }
}