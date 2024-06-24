namespace InformationSystemHZS.Services;

public static class DistanceService
{
    /// <summary>
    /// This uses Pythagorean theorem to calculate distances between 2 positions.
    /// You can refactor this method and change the parameters to better suit your models.
    /// </summary>
    public static double CalculateDistance(int firstX, int firstY, int secondX, int secondY)
    {
        var deltaX = secondX - firstX;
        var deltaY = secondY - firstY;

        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    /// <summary>
    /// Calculates the consumed fuel based on distance (in units) and average fuel consumption (in liters/unit).
    /// </summary>
    public static double CalculateFuelConsumed(double distance, double fuelConsumption)
    {
        return distance * fuelConsumption;
    }

    /// <summary>
    /// Calculates the time in seconds it takes to travel the given distance with a given average speed.
    /// </summary>
    public static double CalculateTimeTaken(double distance, double speed)
    {
        return distance / speed;
    }

    /// <summary>
    /// Calculates the travelled distance in units where <time> are seconds and <speed> units per second.
    /// </summary>
    public static double CalculateDistanceTravelled(double time,  double speed)
    {
        return time * speed;
    }
}