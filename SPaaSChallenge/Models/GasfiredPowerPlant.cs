namespace SPaaSChallenge.Models;

public class GasfiredPowerPlant : IPowerPlant
{
    private readonly double _gasInEuroMWh;
    private readonly double _efficiency;

    public GasfiredPowerPlant(string name, double efficiency, double minimumProduction, double maximumProduction, double gasInEuroMWh)
    {
        _gasInEuroMWh = gasInEuroMWh;
        Name = name;
        _efficiency = efficiency;
        MinimumProduction = minimumProduction;
        MaximumProduction = maximumProduction;
    }

    public string Name { get; }
    public double MinimumProduction { get; }
    public double MaximumProduction { get; }
    public double Cost => _gasInEuroMWh / _efficiency;
}