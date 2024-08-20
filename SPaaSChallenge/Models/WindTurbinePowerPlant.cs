namespace SPaaSChallenge.Models;

public class WindTurbinePowerPlant(string name, double production, double windPercentage) : IPowerPlant
{
    public string Name { get; } = name;

    public double MinimumProduction => production * (windPercentage / 100);

    public double MaximumProduction => production * (windPercentage / 100);

    public double Cost => 0;
}