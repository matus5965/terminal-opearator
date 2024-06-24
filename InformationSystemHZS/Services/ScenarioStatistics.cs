using InformationSystemHZS.Models;

namespace InformationSystemHZS.Services
{
    public class ScenarioStatistics
    {
        public readonly int TotalFireEnginesCount;
        public readonly string ClosestToHospital;
        public readonly string FastestVehicleUnit;
        public readonly string StationWithMostPersonel;
        public readonly List<string> VehiclesByFuelConsumption;
        public readonly string MostBusyUnit;
        public readonly string MostFuelConsumedUnit;

        public ScenarioStatistics(List<Station> stations)
        {
            TotalFireEnginesCount = StatisticsService.GetTotalFireEnginesCount(stations);
            ClosestToHospital = StatisticsService.GetClosestToHospital(stations);
            FastestVehicleUnit = StatisticsService.GetFastestVehicleUnit(stations);
            StationWithMostPersonel = StatisticsService.GetStationWithMostPersonel(stations);
            VehiclesByFuelConsumption = StatisticsService.GetVehiclesByFuelConsumption(stations);
            MostBusyUnit = StatisticsService.GetMostBusyUnit(stations);
            MostFuelConsumedUnit = StatisticsService.MostFuelConsumedUnit(stations);
        }
    }
}
