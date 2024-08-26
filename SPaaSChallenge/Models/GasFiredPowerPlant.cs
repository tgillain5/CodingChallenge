namespace SPaaSChallenge.Models;

public class GasFiredPowerPlant(
    string name,
    double efficiency,
    double minimumProduction,
    double maximumProduction,
    double gasInEuroMWh)
    : IPowerPlant
{
    public string Name { get; } = name;
    public double MinimumProduction { get; } = minimumProduction;
    public double MaximumProduction { get; } = maximumProduction;
    public double Cost => gasInEuroMWh / efficiency;
    public double Production { get; set; }
    public bool IsValidDistribution { get; set; }
}