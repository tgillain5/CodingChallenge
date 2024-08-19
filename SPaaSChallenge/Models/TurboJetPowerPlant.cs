namespace SPaaSChallenge.Models;

public class TurboJetPowerPlant : IPowerPlant
{
    private readonly double _kerosineInEuroMWh;
    private readonly double _efficiency;

    public string Name { get; }
    public double MinimumProduction { get; }
    public double MaximumProduction { get; }
    public double Cost => _kerosineInEuroMWh / _efficiency;
    
    public TurboJetPowerPlant(string name, double efficiency, double minimumProduction, double maximumProduction, double kerosineInEuroMWh)
    {
        _kerosineInEuroMWh = kerosineInEuroMWh;
        Name = name;
        _efficiency = efficiency;
        MinimumProduction = minimumProduction;
        MaximumProduction = maximumProduction;
    }


}