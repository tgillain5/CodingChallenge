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
            computeProduction : powerPlantCurrentNode =>
            {
                powerPlantCurrentNode.Production = powerPlantCurrentNode.MaximumProduction;
                var remainingLoad = _load - powerPlantCurrentNode.GetParentsLoad();
                
                AdaptProductionOnFinalElement(powerPlantCurrentNode, remainingLoad);
                AdaptProductionOnLastElements(powerPlantCurrentNode, remainingLoad);
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
    
    private void AdaptProductionOnLastElements(PowerPlantTree<IPowerPlant> currentNode, double remainingLoad)
    {
        if (currentNode.Parent == null || remainingLoad >= currentNode.MinimumProduction) 
            return;
        
        //we can remove expected load from parent
        if (remainingLoad + (currentNode.Parent.Production - currentNode.Parent.MinimumProduction) > currentNode.MinimumProduction)
        {
            currentNode.Parent.Production -= currentNode.MinimumProduction - remainingLoad;
            currentNode.Production = currentNode.MinimumProduction;
            currentNode.IsValidDistribution = true;

            StoreBranchIfBetterCost(currentNode);
        }
    }

    private void AdaptProductionOnFinalElement(PowerPlantTree<IPowerPlant> currentNode, double remainingLoad)
    {
        if (remainingLoad <= currentNode.MaximumProduction && remainingLoad >= currentNode.MinimumProduction)
        {
            currentNode.Production =  remainingLoad;
            currentNode.IsValidDistribution = true;

            StoreBranchIfBetterCost(currentNode);
        }
    }

    private void StoreBranchIfBetterCost(PowerPlantTree<IPowerPlant> currentNode)
    {
        var branchCost = currentNode.GetTotalCost();
        if (branchCost > _bestCost)
            return;
        
        _distribution.Clear();
        _distribution.AddRange(currentNode.GetBranch()
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