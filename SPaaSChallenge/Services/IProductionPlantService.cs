using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Models;

namespace SPaaSChallenge.Services;

public interface IProductionPlantService
{
    public IEnumerable<Distribution> ComputeLoadDistribution(double load, FuelDto fuelDto, PowerplantDto[] powerPlantDtos);
}