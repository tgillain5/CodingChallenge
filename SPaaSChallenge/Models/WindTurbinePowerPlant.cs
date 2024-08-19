namespace SPaaSChallenge.Models;

public class WindTurbinePowerPlant : IPowerPlant
{
    private readonly double _windPercentage;
    private readonly double _efficiency;
    private readonly double _production;

    private static double MwhPrice => 0;

    public WindTurbinePowerPlant(string name, double efficiency, double production, double windPercentage)
    {
        Name = name;
        _efficiency = efficiency;
        _windPercentage = windPercentage;
        _production = production;
    }

    public string Name { get; }

    public double MinimumProduction => _production * (_windPercentage / 100);

    public double MaximumProduction => _production * (_windPercentage / 100);

    public double Cost => MwhPrice / _efficiency;
}