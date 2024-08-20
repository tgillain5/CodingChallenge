using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Models;

namespace SPaaSChallenge.Services;

public class ProductionPlantService : IProductionPlantService
{
    public List<Distribution> ComputeLoadDistribution(double load, FuelDto fuelDto, PowerplantDto[] powerPlantDtos)
    {

        var electricalNetwork = new ElectricalNetwork(load, powerPlantDtos.Select(x => PowerPlantFactory.Create(x, fuelDto)));
        return electricalNetwork.Distributions;
    }





}