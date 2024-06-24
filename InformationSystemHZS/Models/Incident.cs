using InformationSystemHZS.Models.HelperModels;
using InformationSystemHZS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystemHZS.Models
{
    public enum IncidentType
    {
        FIRE,
        ACCIDENT,
        DISASTER,
        HAZARD,
        TECHNICAL,
        RESCUE
    }

    public class Incident
    {
        public readonly IncidentType Type;
        public bool Resolved { get; private set; }
        public readonly int X;
        public readonly int Y;
        public readonly string Description;
        public readonly int TimeToResolve;
        public readonly string IncidentStartTime;
        public Unit? AssignedUnit { get; private set; }

        private static readonly int _typePaddingSize = Utils.GetTypePaddingSize<IncidentType>();

        public Incident(IncidentType type, int x, int y, string description, string incidentStartTime)
        {
            Type = type;
            Resolved = false;
            X = x;
            Y = y;
            Description = description;

            TimeToResolve = ResolveTimeByIncidentType(type);
            IncidentStartTime = incidentStartTime;
        }

        public void AssignUnit(Unit unit)
        {
            AssignedUnit = unit;
        }

        public void ResolveIncident()
        {
            Resolved = true;
        }

        private static int ResolveTimeByIncidentType(IncidentType incidentType)
        {
            switch (incidentType)
            {
                case IncidentType.FIRE: return 10;
                case IncidentType.ACCIDENT: return 6;
                case IncidentType.DISASTER: return 8;
                case IncidentType.HAZARD: return 10;
                case IncidentType.TECHNICAL: return 4;
                case IncidentType.RESCUE: return 5;
                default: return 0;
            }
        }

        public static IncidentType? FromStringIncidentType(string str)
        {
            switch (str)
            {
                case "FIRE": return IncidentType.FIRE;
                case "ACCIDENT": return IncidentType.ACCIDENT;
                case "DISASTER": return IncidentType.DISASTER;
                case "HAZARD": return IncidentType.HAZARD;
                case "TECHNICAL": return IncidentType.TECHNICAL;
                case "RESCUE": return IncidentType.RESCUE;
                default: return null;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Type).Append(new string(' ', _typePaddingSize - Type.ToString().Length)).Append(" | ");
            sb.Append($"({X:D2}, {Y:D2})").Append(" | ");
            sb.Append(IncidentStartTime).Append(" | ");
            if (AssignedUnit != null && AssignedUnit.AssignedStation != null)
            {
                sb.Append("Assigned to ").Append(AssignedUnit.AssignedStation.Callsign).Append(":");
                sb.Append(AssignedUnit.Callsign).Append(" | ");
            }

            sb.Append(Description);
            return sb.ToString();
        }
    }
}
