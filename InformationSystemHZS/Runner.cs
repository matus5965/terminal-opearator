using InformationSystemHZS.IO;
using InformationSystemHZS.IO.Helpers.Interfaces;
using InformationSystemHZS.Services;
using InformationSystemHZS.ImportExceptions;
using InformationSystemHZS.Models;

using Timer = System.Timers.Timer;
using InformationSystemHZS.Models.HelperModels;
using InformationSystemHZS.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.CompilerServices;

namespace InformationSystemHZS;

public class Runner
{
    private static Timer? _updateTimer;
    private static CallsignEntityMap<Station>? _stations = null;

    public static Task Main(IConsoleManager consoleManager, string entryFileName = "Brnoslava.json")
    {
        // ---- DO NOT TOUCH ----
        var commandParser = new CommandParser(consoleManager);
        var outputWriter = new OutputWriter(consoleManager);
        StartUpdateFunction();
        // ^^^^^ DO NOT TOUCH ^^^^^

        commandParser.Input += Logger.OnInputGiven;
        // Load initial data from JSON
        ScenarioObjectDto? data = null;

        try
        {
            data = ScenarioLoader.GetInitialScenarioData(entryFileName);
        }
        catch 
        {
            outputWriter.ImportExceptionOccured();
            return Task.CompletedTask;
        }

        // Scenario can not be empty
        if (data == null)
        {
            throw new EmptyScenarioException();
        }

        // Throws ImportException if scenario contains invalid data
        CallsignEntityMap<Station> stations = InstanciateStations(data.Stations);
        _stations = stations;
        List<Incident> incidentsHistory = InstanciateIncidentsHistory(stations, data.IncidentsHistory);
        OperatorActions operatorActions = new OperatorActions(stations, incidentsHistory, outputWriter);

        while (true) 
        {
            Command? command = commandParser.GetCommand(operatorActions, outputWriter);
            if (command == null)
            {
                outputWriter.UnknownCommand();
                continue;
            }

            command.Execute();
        }
    }

    /// <summary>
    /// Starts the update function to update function on program start.
    /// DO NOT CHANGE THIS METHOD.
    /// </summary>
    private static void StartUpdateFunction()
    {
        // Set up a timer to call the Update function every second
        _updateTimer = new Timer(TimeSpan.FromSeconds(1));
        _updateTimer.Elapsed += (sender, e) => UpdateFunction();
        _updateTimer.Start();
    }

    /// <summary>
    /// Is called every second to update the state of the system and its objects.
    /// </summary>
    private static void UpdateFunction()
    {
        if (_stations == null) return;

        DateTime currentTime = DateTime.Now;
        foreach (Station station in _stations.GetAllEntities())
        {
            foreach (Unit unit in station.GetAllUnits())
            {
                unit.UpdateState(currentTime);
            }
        }
    }

    private static CallsignEntityMap<Station> InstanciateStations(List<StationDto> stationsDto)
    {
        CallsignEntityMap<Station> stations = new CallsignEntityMap<Station>("S");

        foreach (var stationDto in stationsDto)
        {
            Station station = new Station(stationDto.Callsign, stationDto.Name,
                                          stationDto.PositionDto.X, stationDto.PositionDto.Y);
            if (!stations.SafelyAddEntity(station, station.Callsign))
            {
                throw new DuplicitIDException("stations");
            }

            InstanciateStationUnits(station, stationDto.Units);
        }

        return stations;
    }

    private static void InstanciateStationUnits(Station station, List<UnitDto> unitsDto)
    {
        foreach (var unitDto in unitsDto)
        {
            VehicleDto vehicleDto = unitDto.VehicleDto;
            VehicleType? vehicleType = Vehicle.FromStringVehicleType(vehicleDto.Type);
            if (vehicleType == null)
            {
                throw new InvalidVehicleTypeException();
            }

            Vehicle vehicle = new Vehicle(vehicleDto.Name, vehicleType.Value, vehicleDto.FuelConsumption,
                                          vehicleDto.Speed, vehicleDto.Capacity);
            Unit? unit = station.AssignUnit(unitDto.Callsign, vehicle);
            if (unit == null)
            {
                throw new DuplicitIDException("units");
            }

            InstanciateUnitMembers(unit, unitDto.Members);
        }
    }

    private static void InstanciateUnitMembers(Unit unit, List<MemberDto> membersDto)
    {
        if (membersDto.Count == 0 || membersDto.Count > unit.UsedVehicle.Capacity)
        {
            throw new InvalidMembersCountException();
        }

        foreach (var memberDto in membersDto)
        {
            Member? member = unit.AssignMember(memberDto.Callsign, memberDto.Name);
            if (member == null)
            {
                throw new DuplicitIDException("members");
            }
        }
    }

    private static List<Incident> InstanciateIncidentsHistory(CallsignEntityMap<Station> stations, List<RecordedIncidentDto> incidentsDto)
    {
        List<Incident> incidents = new List<Incident>();

        foreach (var incidentDto in incidentsDto)
        {
            IncidentType? incidentType = Incident.FromStringIncidentType(incidentDto.Type);
            if (incidentType == null)
            {
                throw new RecordedIncidentException("Trying to load scenario with recorded incident of unknown type");
            }

            Incident incident = new Incident(incidentType.Value, incidentDto.Location.X, incidentDto.Location.Y,
                                             incidentDto.Description, incidentDto.IncidentStartTIme);
            Station? station = stations.GetEntity(incidentDto.AssignedStation);
            if (station == null)
            {
                throw new RecordedIncidentException("Trying to load scenario with recorded incident assigned to non-existing station");
            }

            Unit? unit = station.GetUnit(incidentDto.AssignedUnit);
            if (unit == null)
            {
                throw new RecordedIncidentException("Trying to load scenario with recorded incident assigned to non-existing unit");
            }

            incidents.Add(incident);
            unit.AddRecordedIncident(incident);
        }

        return incidents;
    }
}