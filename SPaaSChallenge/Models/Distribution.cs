namespace SPaaSChallenge.Models;

public class Distribution(string powerPlantName, double production)
{
    public string PowerPlantName { get; } = powerPlantName;

    public double Production { get; } = production;
}