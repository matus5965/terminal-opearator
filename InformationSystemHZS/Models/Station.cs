using InformationSystemHZS.Collections;
using InformationSystemHZS.Models.Interfaces;
using InformationSystemHZS.Services;
using System.Text;

namespace InformationSystemHZS.Models
{
    public class Station : IModel
    {
        public string Callsign { get; private set; }
        public readonly string Name;
        private static int _namePaddingSize = 0;
        public readonly int X;
        public readonly int Y;

        private readonly CallsignEntityMap<Unit> _units;

        public Station(string callsign, string name, int x, int y)
        {
            Callsign = callsign;
            Name = name;
            if (name.Length > _namePaddingSize)
            {
                _namePaddingSize = name.Length;
            }

            X = x;
            Y = y;
            _units = new CallsignEntityMap<Unit>("J");
        }

        public Unit? AssignUnit(string unitCallsign, Vehicle vehicle)
        {
            if (!Unit.ValidCallsign(unitCallsign))
            {
                return null;
            }

            Unit newUnit = new Unit(unitCallsign, this, vehicle);
            return _units.SafelyAddEntity(newUnit, unitCallsign) ? newUnit : null;
        }

        public bool AssignUnit(Unit unit)
        {
            bool addingIsSuccesful = _units.SafelyAddEntity(unit, null);

            if (addingIsSuccesful)
            {
                unit.Reassign(_units.GetMaxCallsign(), this);
            }

            return addingIsSuccesful;
        }

        public bool ReassignUnit(Station? newStation, string unitCallsign)
        {
            if (!Unit.ValidCallsign(unitCallsign))
            {
                return false;
            }

            Unit? unit = _units.GetEntity(unitCallsign);
            if (unit == null)
            {
                return false;
            }

            if (newStation == null || newStation.AssignUnit(unit))
            {
                _units.SafelyRemoveEntity(unitCallsign);
                return true;
            }

            return false;
        }

        public Unit? GetUnit(string callsign)
        {
            return _units.GetEntity(callsign);
        }

        public List<Unit> GetAllUnits()
        {
            return _units.GetAllEntities();
        }

        public int GetUnitsCount()
        {
            return _units.GetEntitiesCount();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Callsign).Append(" | ");
            sb.Append(Name).Append(new string(' ', _namePaddingSize - Name.Length)).Append(" | ");
            sb.Append(_units.GetEntitiesCount()).Append(" units | ");
            sb.Append($"({X:D2}, {Y:D2})");
            return sb.ToString();
        }

        public static bool ValidCallsign(string callsign)
        {
            if (callsign.Length != 1 + CallsignEntityMap<Station>.CallsignDigitPlaces)
            {
                return false;
            }

            int numIdentifier;
            if (callsign[0] != 'S' || !int.TryParse(callsign.Substring(1), out numIdentifier))
            {
                return false;
            }

            return true;

        }
    }
}
