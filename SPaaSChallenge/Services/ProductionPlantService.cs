using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Models;

namespace SPaaSChallenge.Services;

public class ProductionPlantService : IProductionPlantService
{
    public List<Distribution> ComputeLoadDistribution(double load, FuelsDto fuelsDto, PowerplantDto[] powerPlantDtos)
    {

        var electricalNetwork = new ElectricalNetwork(load, powerPlantDtos.Select(x => PowerPlantFactory.Create(x, fuelsDto)));
        
        return electricalNetwork.distributions;
    }





}