using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Models;
using SPaaSChallenge.Services.Helpers;

namespace SPaaSChallenge.Services;

public class ProductionPlantService(IPowerPlantFactory powerPlantFactory, ILogger<ProductionPlantService> logger) : IProductionPlantService
{
    private double _load;
    private IEnumerable<IPowerPlant> _availablePowerPlants;
    private double _bestCost = double.MaxValue;
    private readonly List<Distribution> _distribution = [];
    
    public IEnumerable<Distribution> ComputeLoadDistribution(double load, FuelDto fuelDto, PowerplantDto[] powerPlantDtos)
    {
        _load = load;
        _availablePowerPlants = powerPlantDtos.Select(x => powerPlantFactory.Create(x, fuelDto));
        
        ValidateDistributionPossibilityExists();
        
        logger.LogInformation("starting to build distribution");

        new PowerPlantTree<IPowerPlant>(null).BuildDistribution(
            _availablePowerPlants.OrderBy(x => x.Cost),
            computeProduction : x =>
            {
                x.Element.Production = x.Element.MaximumProduction;
                var remainingLoad = _load - x.GetParentsLoad();
                
                AdaptProductionOnFinalElement(x, remainingLoad);
                AdaptProductionOnLastElements(x, remainingLoad);
            });

        AppendUnusedPowerPlant();
        
        return _distribution;
    }
    
    private void ValidateDistributionPossibilityExists()
    {
        if (!_availablePowerPlants.Any(x => x.MinimumProduction < _load))
            throw new DistributionImpossibleException("Impossible to provide the expected load");

        if (_availablePowerPlants.Sum(x => x.MaximumProduction) < _load)
            throw new DistributionImpossibleException("Impossible to provide the expected load");
    }
    
    private void AdaptProductionOnLastElements(PowerPlantTree<IPowerPlant> x, double remainingLoad)
    {
        if (x.Parent.Element == null || remainingLoad >= x.Element.MinimumProduction) 
            return;
        
        //we can remove expected load from parent
        if (remainingLoad + (x.Parent.Element.Production - x.Parent.Element.MinimumProduction) > x.Element.MinimumProduction)
        {
            x.Parent.Element.Production -= x.Element.MinimumProduction - remainingLoad;
            x.Element.Production = x.Element.MinimumProduction;
            x.Element.IsValidDistribution = true;

            StoreBranchIfBetterCost(x);
        }
    }

    private void AdaptProductionOnFinalElement( PowerPlantTree<IPowerPlant> x, double remainingLoad)
    {
        if (remainingLoad <= x.Element.MaximumProduction && remainingLoad >= x.Element.MinimumProduction)
        {
            x.Element.Production =  remainingLoad;
            x.Element.IsValidDistribution = true;

            StoreBranchIfBetterCost(x);
        }
    }

    private void StoreBranchIfBetterCost(PowerPlantTree<IPowerPlant> x)
    {
        var branchCost = x.GetTotalCost();
        if (branchCost > _bestCost)
            return;
        
        _distribution.Clear();
        _distribution.AddRange(x.GetBranch()
            .Select(item => new Distribution(powerPlantName: item.Name, production: item.Production))
            .Reverse());
        
        _bestCost = branchCost;
    }

    private void AppendUnusedPowerPlant()
    {
        _distribution
            .AddRange(_availablePowerPlants
                .OrderBy(x => x.Cost)
                .Where(item => _distribution.All(x => x.PowerPlantName != item.Name))
                .Select(item => new Distribution(powerPlantName: item.Name, production: 0)));
    }
    
}