using SPaaSChallenge.Models;

namespace SPaaSChallenge.Services.Helpers;

public interface IDistributionBuilder
{
    IDistributionBuilder SetLoad(double load);
    IDistributionBuilder SetPowerPlants(IEnumerable<IPowerPlant> powerPlants);
    IDistributionBuilder ValidateDistributionPossibilityExists();
    List<Distribution> Build();
}