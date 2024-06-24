using InformationSystemHZS.Collections;
using InformationSystemHZS.IO;
using InformationSystemHZS.Models.HelperModels;
using InformationSystemHZS.Models.Interfaces;
using InformationSystemHZS.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InformationSystemHZS.Models
{
    public enum UnitState
    {
        AVAILABLE,
        EN_ROUTE,
        ON_SITE,
        RETURNING
    }

    public class Unit : IModel
    {
        public string Callsign { get; private set; }
        public Station? AssignedStation { get; private set; }
        public readonly Vehicle UsedVehicle;

        public UnitState State { get; private set; }
        public Incident? CurrentIncident { get; private set; }
        public int ResolvedIncidents { get; private set; }
        public TimeSpan BusyTime { get; private set; }
        public double ConsumedFuel { get; private set; }

        private TimeSpan _timeToGetOnSite;
        private TimeSpan _timeToResolveIncident;
        private TimeSpan _timeToReturnOnStation;
        private DateTime _lastUpdate;
        private double _toConsume;

        private static int _vehicleNamePaddingSize = 0;
        private static readonly int _statePaddingSize = Utils.GetTypePaddingSize<UnitState>();
        private readonly CallsignEntityMap<Member> _members;

        public Unit(string callsign, Station? station, Vehicle vehicle)
        {
            Callsign = callsign;
            AssignedStation = station;
            UsedVehicle = vehicle;
            if (vehicle.Name.Length > _vehicleNamePaddingSize)
            {
                _vehicleNamePaddingSize = vehicle.Name.Length;
            }

            _members = new CallsignEntityMap<Member>("H");
            State = UnitState.AVAILABLE;
            ResolvedIncidents = 0;
            BusyTime = new TimeSpan();
            ConsumedFuel = 0;
        }

        public Member? AssignMember(string memberCallsign, string name)
        {
            if (!Member.ValidCallsign(memberCallsign) || UsedVehicle.Capacity == _members.GetEntitiesCount())
            {
                return null;
            }

            Member newMember = new Member(memberCallsign, this, name);
            return _members.SafelyAddEntity(newMember, memberCallsign) ? newMember : null;
        }

        public bool AssignMember(Member member)
        {
            if (UsedVehicle.Capacity == _members.GetEntitiesCount())
            {
                return false;
            }

            bool addingIsSuccesful = _members.SafelyAddEntity(member, null);
            if (addingIsSuccesful)
            {
                member.Reassign(_members.GetMaxCallsign(), this);
            }

            return addingIsSuccesful;
        }

        public bool ReassignMember(Unit? newUnit, string memberCallsign)
        {
            if (_members.GetEntitiesCount() <= 1 || !Member.ValidCallsign(memberCallsign))
            {
                return false;
            }

            Member? member = _members.GetEntity(memberCallsign);
            if (member == null)
            {
                return false;
            }

            if (newUnit == null || newUnit.AssignMember(member))
            {
                _members.SafelyRemoveEntity(memberCallsign);
                return true;
            }

            return false;
        }

        public Member? GetMember(string callsign)
        {
            return _members.GetEntity(callsign);
        }

        public List<Member> GetAllMembers()
        {
            return _members.GetAllEntities();
        }

        public int GetMembersCount()
        {
            return _members.GetEntitiesCount();
        }

        public bool IsFull()
        {
            return _members.GetEntitiesCount() == UsedVehicle.Capacity;
        }

        public bool AssignIncident(Incident incident, DateTime startTime)
        {
            if (AssignedStation == null || State != UnitState.AVAILABLE || !UsedVehicle.IsCompatibleWithIncident(incident.Type))
            {
                return false;
            }

            CurrentIncident = incident;
            incident.AssignUnit(this);
            State = UnitState.EN_ROUTE;

            double distance = DistanceService.CalculateDistance(AssignedStation.X, AssignedStation.Y, incident.X, incident.Y);
            double realTime = DistanceService.CalculateTimeTaken(distance, UsedVehicle.Speed);

            BusyTime = TimeSpan.Zero;
            _timeToGetOnSite = new TimeSpan(0, 0, 0, 0, (int) (realTime * 1000));
            _timeToResolveIncident = TimeSpan.FromSeconds(incident.TimeToResolve);
            _timeToReturnOnStation = _timeToGetOnSite; // TimeSpan is immutable, so same reference is no problem
            _lastUpdate = startTime;

            
            _toConsume = DistanceService.CalculateFuelConsumed(2 * distance, UsedVehicle.FuelConsumption);

            // Console.WriteLine($"To site: {_timeToGetOnSite.TotalSeconds}. To resolve: {_timeToResolveIncident.TotalSeconds}. To return: {_timeToReturnOnStation.TotalSeconds}");
            return true;
        }

        public void Reassign(string callsign, Station station)
        {
            Callsign = callsign;
            AssignedStation = station;
        }

        public bool AddRecordedIncident(Incident incident)
        {
            if (AssignedStation == null) return false;
            
            incident.AssignUnit(this);

            double distance = 2 * DistanceService.CalculateDistance(AssignedStation.X, AssignedStation.Y, incident.X, incident.Y);
            ConsumedFuel += DistanceService.CalculateFuelConsumed(distance, UsedVehicle.FuelConsumption);

            incident.AssignUnit(this);
            incident.ResolveIncident();

            ResolvedIncidents++;
            return true;
        }

        public void UpdateState(DateTime currentTime) // I do not assume time will be exactly in MM:SS:000 form
        {
            if (CurrentIncident == null || State == UnitState.AVAILABLE) return;

            TimeSpan timeToCheck = currentTime - _lastUpdate;
            _lastUpdate = currentTime;
            BusyTime += timeToCheck;

            if (State == UnitState.EN_ROUTE)
            {
                TimeSpan min = (timeToCheck >= _timeToGetOnSite) ? _timeToGetOnSite : timeToCheck;
                _timeToGetOnSite -= min;
                timeToCheck -= min;

                if (_timeToGetOnSite <= TimeSpan.Zero)
                {
                    State = UnitState.ON_SITE;
                }
            }

            if (State == UnitState.ON_SITE)
            {
                TimeSpan min = (timeToCheck >= _timeToResolveIncident) ? _timeToResolveIncident : timeToCheck;
                _timeToResolveIncident -= min;
                timeToCheck -= min;

                if (_timeToResolveIncident <= TimeSpan.Zero)
                {
                    State = UnitState.RETURNING;
                }
            }

            if (State == UnitState.RETURNING)
            {
                TimeSpan min = (timeToCheck >= _timeToReturnOnStation) ? _timeToReturnOnStation : timeToCheck;
                _timeToReturnOnStation -= min;
                timeToCheck -= min;

                if (_timeToReturnOnStation <= TimeSpan.Zero)
                {
                    ConsumedFuel += _toConsume;
                    State = UnitState.AVAILABLE;
                    CurrentIncident.ResolveIncident();
                    CurrentIncident = null;
                    ResolvedIncidents++;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (AssignedStation != null)
            {
                sb.Append(AssignedStation.Callsign).Append(" | ");
            }

            sb.Append(Callsign).Append(" | ");
            sb.Append(UsedVehicle.Name).Append(new string(' ', _vehicleNamePaddingSize - UsedVehicle.Name.Length)).Append(" | ");
            sb.Append(_members.GetEntitiesCount()).Append("/").Append(UsedVehicle.Capacity).Append(" | ");
            sb.Append(State).Append(new string(' ', _statePaddingSize - State.ToString().Length));
            if (State != UnitState.AVAILABLE)
            {
                sb.Append(" | ").Append(Utils.MinutesAndSeconds(BusyTime.Seconds));
            }

            return sb.ToString();
        }

        public static bool ValidCallsign(string callsign)
        {
            if (callsign.Length != 1 + CallsignEntityMap<Unit>.CallsignDigitPlaces)
            {
                return false;
            }

            int numIdentifier;
            if (callsign[0] != 'J' || !int.TryParse(callsign.Substring(1), out numIdentifier))
            {
                return false;
            }

            return true;

        }
    }
}
