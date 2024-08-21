using SPaaSChallenge.Models;

namespace SPaaSChallenge.Controllers.Helpers;

public interface IDistributionBuilder
{
    DistributionBuilder SetLoad(double load);
    DistributionBuilder SetPowerPlants(IEnumerable<IPowerPlant> powerPlants);
    DistributionBuilder ValidateDistributionPossibilityExists();
    List<Distribution> Build();
}