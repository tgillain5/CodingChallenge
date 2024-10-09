using Microsoft.AspNetCore.Mvc;
using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Controllers.Mappers;
using SPaaSChallenge.Services;


namespace SPaaSChallenge.Controllers;

[ApiController]
public class ProductionPlanController(IProductionPlantService productionPlantService)
{
    
    [HttpPost("productionplan")]
    public  ActionResult<(string name, double production)[]> ProductionPlan(ProductionPlantRequest productionPlantRequest)
    {
        var distributions = productionPlantService.ComputeLoadDistribution(
            load: productionPlantRequest.Load,
            fuelDto: productionPlantRequest.FuelDto,
            powerPlantDtos: productionPlantRequest.PowerPlants);
        return new OkObjectResult(distributions.Select(x=>x.Map()).ToArray());

    }
}