using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Models;
using SPaaSChallenge.Services.Helpers;

namespace SPaaSChallenge.Services;

public class ProductionPlantService(IDistributionBuilder distributionBuilder,IPowerPlantFactory powerPlantFactory) : IProductionPlantService
{
    public List<Distribution> ComputeLoadDistribution(double load, FuelDto fuelDto, PowerplantDto[] powerPlantDtos)
    {
        return distributionBuilder
            .SetLoad(load)
            .SetPowerPlants(powerPlantDtos.Select(x => powerPlantFactory.Create(x, fuelDto)))
            .ValidateDistributionPossibilityExists()
            .Build();
    }
    
}