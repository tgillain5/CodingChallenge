using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Controllers.Helpers;
using SPaaSChallenge.Models;

namespace SPaaSChallenge.Services;

public class ProductionPlantService(IDistributionBuilder distributionBuilder,IPowerPlantFactory powerPlantFactory) : IProductionPlantService
{
    public List<Distribution> ComputeLoadDistribution(double load, FuelDto fuelDto, PowerplantDto[] powerPlantDtos)
    {
        var distributions = distributionBuilder
            .SetLoad(load)
            .SetPowerPlants(powerPlantDtos.Select(x => powerPlantFactory.Create(x, fuelDto)))
            .ValidateDistributionPossibilityExists()
            .Build();

        return distributions;
    }
    
}