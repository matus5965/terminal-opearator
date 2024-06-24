using InformationSystemHZS.Collections;
using InformationSystemHZS.Models.Interfaces;

namespace InformationSystemHZS.Models
{
    public class Member : IModel
    {
        public string Callsign { get; private set; }
        public Unit? AssignedUnit;
        public string Name;

        public Member(string callsign, Unit? unit, string name)
        {
            Callsign = callsign;
            AssignedUnit = unit;
            Name = name;
        }

        public void Reassign(string callsign, Unit unit)
        {
            Callsign = callsign;
            AssignedUnit = unit;
        }

        public static bool ValidCallsign(string callsign)
        {
            if (callsign.Length != 1 + CallsignEntityMap<Member>.CallsignDigitPlaces)
            {
                return false;
            }

            int numIdentifier;
            if (callsign[0] != 'H' || !int.TryParse(callsign.Substring(1), out numIdentifier))
            {
                return false;
            }

            return true;

        }
    }
}
