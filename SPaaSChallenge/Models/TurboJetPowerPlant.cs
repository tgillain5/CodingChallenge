namespace SPaaSChallenge.Models;

public class TurboJetPowerPlant(
    string name,
    double efficiency,
    double minimumProduction,
    double maximumProduction,
    double kerosineInEuroMWh)
    : IPowerPlant
{
    public string Name { get; } = name;
    public double MinimumProduction { get; } = minimumProduction;
    public double MaximumProduction { get; } = maximumProduction;
    public double Cost => kerosineInEuroMWh / efficiency;
    public double Production { get; set; }
    public bool IsValidDistribution { get; set; }
}