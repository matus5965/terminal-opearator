using InformationSystemHZS.Collections;
using InformationSystemHZS.IO;
using InformationSystemHZS.Models;
using InformationSystemHZS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystemHZS.Services
{
    public class OperatorActions
    {
        private CallsignEntityMap<Station> _stations;
        private List<Incident> _incidentHistory;
        private OutputWriter _outputWriter;

        public string[] StringActions =
        {
            "list-stations",
            "list-units",
            "list-incidents",
            "add-member",
            "remove-member",
            "reassign-member",
            "reassign-unit",
            "report",
            "statistics"
        };

        public Action<string[]>[] Actions;

        public OperatorActions(CallsignEntityMap<Station> stations, List<Incident> incidentHistory, OutputWriter output)
        {
            _stations = stations;
            _incidentHistory = incidentHistory;
            _outputWriter = output;
            Actions = new Action<string[]>[]
            {
                ListStations,
                ListUnits,
                ListIncidents,
                AddMember,
                RemoveMember,
                ReassignMember,
                ReassignUnit,
                Report,
                Statistics
            };
        }

        // list-stations
        public void ListStations(string[] args)
        {
            if (args.Length != 0)
            {
                _outputWriter.InvalidArgumentCount("list-stations", args.Length, 0);
                return;
            }

            _outputWriter.ListEntities(_stations.GetAllEntities());
        }

        // list-units
        public void ListUnits(string[] args)
        {
            if (args.Length != 0)
            {
                _outputWriter.InvalidArgumentCount("list-units", args.Length, 0);
                return;
            }

            List<Unit> allUnits = new List<Unit>();
            foreach (Station station in _stations.GetAllEntities())
            {
                List<Unit> stationUnits = station.GetAllUnits();
                allUnits.AddRange(stationUnits);
            }

            _outputWriter.ListEntities(allUnits);
        }

        // list-incidents
        public void ListIncidents(string[] args)
        {
            if (args.Length != 0)
            {
                _outputWriter.InvalidArgumentCount("list-incidents", args.Length, 0);
                return;
            }

            _outputWriter.ListEntities(_incidentHistory.Where(i => !i.Resolved).ToList());
        }

        // Only as IO action. Interacts with OutputWriter(user)
        private IModel? GetEntity(string stationCallsign, string? unitCallsign = null, string? memberCallsign = null, string identifier = "")
        {
            IModel? entity = null;

            entity = _stations.GetEntity(stationCallsign);
            if (entity == null)
            {
                _outputWriter.InvalidArgument($"{identifier}station-id", stationCallsign, $"existing {identifier}station-id");
                return null;
            }

            if (unitCallsign != null)
            {
                entity = ((Station) entity).GetUnit(unitCallsign);
                if (entity == null)
                {
                    _outputWriter.InvalidArgument($"{identifier}unit-id", unitCallsign, $"existing {identifier}unit-id");
                    return null;
                }

                if (memberCallsign != null)
                {
                    entity = ((Unit) entity).GetMember(memberCallsign);
                    if (entity == null)
                    {
                        _outputWriter.InvalidArgument($"{identifier}member-id", unitCallsign, $"existing {identifier}member-id");
                        return null;
                    }
                }
            }

            return entity;
        }

        // add-member <station-id> <unit-id> <member-name>
        public void AddMember(string[] args)
        {
            if (args.Length != 3)
            {
                _outputWriter.InvalidArgumentCount("add-member", args.Length, 3);
                return;
            }

            Unit? unit = (Unit?) GetEntity(args[0], args[1]);
            if (unit == null) return;

            string memberName = args[2];
            if (memberName.Length == 0)
            {
                _outputWriter.InvalidArgument("memeber-name", memberName, "non-empty string");
                return;
            }

            if (unit.IsFull())
            {
                _outputWriter.CapacityError("vehicle capacity is full");
                return;
            }

            unit.AssignMember(new Member("-", null, memberName));
            _outputWriter.EntityAssigned(memberName, null, unit, "unit");
        }

        // remove-member <station-id> <unit-id> <member-id>
        public void RemoveMember(string[] args)
        {
            if (args.Length != 3)
            {
                _outputWriter.InvalidArgumentCount("remove-member", args.Length, 3);
                return;
            }

            Unit? unit = (Unit?) GetEntity(args[0], args[1]);
            if (unit == null) return;

            string memberCallsign = args[2];
            Member? member = unit.GetMember(memberCallsign);

            if (member == null)
            {
                _outputWriter.InvalidArgument("member-id", memberCallsign, "existing member-id");
                return;
            }

            if (unit.GetMembersCount() == 1)
            {
                _outputWriter.CapacityError("unit would be empty");
                return;
            }

            unit.ReassignMember(null, memberCallsign);
            _outputWriter.EntityAssigned(member.Name, unit, null, "unit");
        }

        // reassign-member <station-id> <unit-id> <member-id> <new_station-id> <new_unit-id>
        public void ReassignMember(string[] args)
        {
            if (args.Length != 5)
            {
                _outputWriter.InvalidArgumentCount("reassign-member", args.Length, 5);
                return;
            }

            Unit? oldUnit = (Unit?) GetEntity (args[0], args[1], null, "old_");
            if (oldUnit == null) return;

            Unit? newUnit = (Unit?) GetEntity (args[3], args[4], null, "new_");
            if (newUnit == null) return;

            string memberCallsign = args[2];
            Member? member = oldUnit.GetMember(memberCallsign);
            if (member == null)
            {
                _outputWriter.InvalidArgument("member-id", memberCallsign, "existing member-id");
                return;
            }

            if (oldUnit.GetMembersCount() == 1)
            {
                _outputWriter.CapacityError("old unit would be empty.");
                return;
            }

            if (newUnit.IsFull())
            {
                _outputWriter.CapacityError("new unit vehicle capacity is full.");
                return;
            }

            oldUnit.ReassignMember(newUnit, memberCallsign);
            _outputWriter.EntityAssigned(member.Name, oldUnit, newUnit, "unit");
        }

        // reassign-unit <station-id> <unit-id> <new_station-id>
        public void ReassignUnit(string[] args)
        {
            if (args.Length != 3)
            {
                _outputWriter.InvalidArgumentCount("reassign-unit", args.Length, 3);
                return;
            }

            Station? oldStation = (Station?) GetEntity(args[0], null, null, "old_");
            if (oldStation == null) return;

            Station? newStation = (Station?) GetEntity(args[2], null, null, "new_");
            if (newStation == null) return;

            string unitCallsign = args[1];
            Unit? unit = oldStation.GetUnit(unitCallsign);
            if (unit == null)
            {
                _outputWriter.InvalidArgument("unit-id", unitCallsign, "existing unit-id");
                return;
            }

            oldStation.ReassignUnit(newStation, unitCallsign);
            _outputWriter.EntityAssigned("Unit", oldStation, newStation, "station");
        }

        private Unit? AssignIncidentToClosestUnit(Incident incident)
        {
            List<Station> allStations = _stations.GetAllEntities();
            var distanceGroupedStations = allStations.GroupBy(s => DistanceService.CalculateDistance(s.X, s.Y, incident.X, incident.Y));
            var distanceOrderedStations = distanceGroupedStations.OrderBy(s => s.Key);

            foreach (var stations in distanceOrderedStations)
            {
                List<Unit> units = new List<Unit>();
                foreach (var station in stations)
                {
                    units.AddRange(station.GetAllUnits());
                }

                // u.AssignedStation can not be null here, therefor I am ignoring the warning
                var orderedUnits = units.OrderByDescending(u => u.UsedVehicle.Speed).ThenBy(u => u.AssignedStation.Callsign).ThenBy(u => u.Callsign);

                foreach (Unit unit in orderedUnits)
                {
                    if (unit.AssignIncident(incident, DateTime.Now))
                    {
                        return unit;
                    }
                }
            }

            return null;
        }

        // report <x> <y> <type> <descritpion>
        public void Report(string[] args)
        {
            if (args.Length != 4)
            {
                _outputWriter.InvalidArgumentCount("report", args.Length, 4);
                return;
            }

            int? x = Utils.GetNumber(args[0]);
            int? y = Utils.GetNumber(args[1]);
            if (x == null || x < 0 || x >= 100)
            {
                _outputWriter.InvalidArgument("x", args[0], "integer in range 0 - 99");
                return;
            }

            if (y == null || y < 0 || y >= 100)
            {
                _outputWriter.InvalidArgument("y", args[1], "integer in range 0 - 99");
                return;
            }

            IncidentType? type = Incident.FromStringIncidentType(args[2].ToUpper());
            if (type == null)
            {
                _outputWriter.InvalidArgument("type", args[2], "valid incident type");
                return;
            }

            Incident incident = new Incident(type.Value, x.Value, y.Value, args[3], DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
            Unit? unit = AssignIncidentToClosestUnit(incident);
            if (unit != null)
            {
                _incidentHistory.Add(incident);
                _outputWriter.IncidentAssigned(unit);
            }
            else
            {
                _outputWriter.CapacityError("there is no available unit with vehicle suffiecient for this type of incident");
            }
        }

        public void Statistics(string[] args)
        {
            if (args.Length != 0)
            {
                _outputWriter.InvalidArgumentCount("statistics", args.Length, 0);
                return;
            }

            ScenarioStatistics stats = new ScenarioStatistics(_stations.GetAllEntities());
            _outputWriter.Statistics(stats);
        }
    }
}
