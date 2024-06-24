using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystemHZS.Models
{
    using VehicleIncidentMap = Dictionary<VehicleType, HashSet<IncidentType>>;
    public enum VehicleType
    {
        FIRE_ENGINE,
        TECHNICAL_VEHICLE,
        ANTI_GAS_VEHICLE,
        RESCUE_VEHICLE,
        CRANE_TRUCK
    }

    public class Vehicle
    {
        public readonly string Name;
        public readonly VehicleType Type;
        public readonly double FuelConsumption;
        public readonly int Speed;
        public readonly int Capacity;

        public static readonly VehicleIncidentMap Compatibility = new VehicleIncidentMap()
        {
            { VehicleType.FIRE_ENGINE, new HashSet<IncidentType> { 
                IncidentType.FIRE, IncidentType.ACCIDENT } },
            { VehicleType.TECHNICAL_VEHICLE, new HashSet<IncidentType> { 
                IncidentType.DISASTER, IncidentType.TECHNICAL } },
            { VehicleType.ANTI_GAS_VEHICLE, new HashSet<IncidentType> { 
                IncidentType.HAZARD } },
            { VehicleType.RESCUE_VEHICLE, new HashSet<IncidentType> { 
                IncidentType.ACCIDENT, IncidentType.RESCUE } },
            { VehicleType.CRANE_TRUCK, new HashSet<IncidentType> { 
                IncidentType.TECHNICAL, IncidentType.RESCUE } }
        };

        public Vehicle(string name, VehicleType type, double fuelConsumption, int speed, int capacity)
        {
            Name = name;
            Type = type;
            FuelConsumption = fuelConsumption;
            Speed = speed;
            Capacity = capacity;
        }

        public bool IsCompatibleWithIncident(IncidentType incidentType)
        {
            return Compatibility[Type].Contains(incidentType);
        }

        public static VehicleType? FromStringVehicleType(string str)
        {
            switch (str)
            {
                case "FIRE_ENGINE": return VehicleType.FIRE_ENGINE;
                case "TECHNICAL_VEHICLE": return VehicleType.TECHNICAL_VEHICLE;
                case "ANTI_GAS_VEHICLE": return VehicleType.ANTI_GAS_VEHICLE;
                case "RESCUE_VEHICLE": return VehicleType.RESCUE_VEHICLE;
                case "CRANE_TRUCK": return VehicleType.CRANE_TRUCK;
                default: return null;
            }
        }
    }
}
