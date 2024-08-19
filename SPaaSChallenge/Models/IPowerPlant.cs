namespace SPaaSChallenge.Models;

public interface IPowerPlant
{
    public string Name { get; }

    public double MinimumProduction { get; }

    public double MaximumProduction { get; }

    public double Cost { get; }
}