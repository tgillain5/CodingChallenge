using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Controllers.Mappers;
using SPaaSChallenge.Services;


namespace SPaaSChallenge.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductionPlanController
{
    private readonly IProductionPlantService _productionPlantService;

    public ProductionPlanController(IProductionPlantService productionPlantService)
    {
        _productionPlantService = productionPlantService;
    }

    [HttpPost("productionplan")]
    public  ActionResult<(string name, double production)[]> ProductionPlan(ProductionPlantRequest productionPlantRequest)
    {

        var distributions = _productionPlantService.ComputeLoadDistribution(
            load: productionPlantRequest.load,
            fuelsDto: productionPlantRequest.FuelsDto,
            powerPlantDtos: productionPlantRequest.powerplants);
        return new OkObjectResult(distributions.Select(x=>x.Map()).ToArray());

    }
}