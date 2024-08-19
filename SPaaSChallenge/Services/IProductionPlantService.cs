using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Models;

namespace SPaaSChallenge.Services;

public interface IProductionPlantService
{
    public List<Distribution> ComputeLoadDistribution(double load, FuelsDto fuelsDto, PowerplantDto[] powerPlantDtos);
}