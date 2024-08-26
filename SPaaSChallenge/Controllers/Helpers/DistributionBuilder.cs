using SPaaSChallenge.Models;
using SPaaSChallenge.Services;

namespace SPaaSChallenge.Controllers.Helpers;

public class DistributionBuilder(ILogger<DistributionBuilder> logger) : IDistributionBuilder
{
    private double _load;
    private IEnumerable<IPowerPlant> _availablePowerPlants;
    private double _bestCost = Double.MaxValue;

    
    public DistributionBuilder SetLoad(double load)
    {
        _load = load;
        return this;
    }

    public DistributionBuilder SetPowerPlants(IEnumerable<IPowerPlant> powerPlants)
    {
        _availablePowerPlants = powerPlants;
        return this;
    }

    public DistributionBuilder ValidateDistributionPossibilityExists()
    {
        if (!_availablePowerPlants.Any(x => x.MinimumProduction < _load))
        {
            throw new DistributionImpossibleException("Impossible to provide the expected load");
        }

        if (_availablePowerPlants.Sum(x => x.MaximumProduction) < _load)
        {
            throw new DistributionImpossibleException("Impossible to provide the expected load");
        }

        return this;
    }

    public List<Distribution> Build()
    {
        logger.LogInformation("starting to build distribution");
        List<Distribution> distributions = [];
        var powerPlants = _availablePowerPlants.OrderBy(x => x.Cost).ToList();

        
        var root = new NodeWrapper<IPowerPlant>(null);
        root.AddChildren(
            powerPlants,
            root,
            x => { ComputeProduction(x,distributions); },
            IsFinalInDistribution);

        AppendUnusedPowerPlant(distributions);
        return distributions;
    }

    private void ComputeProduction(NodeWrapper<IPowerPlant> x, List<Distribution> distributions)
    {
        if (x.Element == null)
            return;
                
        x.Element.Production = x.Element.MaximumProduction;
        var remainingLoad = _load - x.GetParentsLoad();
                
        //we have a correct last element and can compute the node production cost
        AdaptLastElementProduction(remainingLoad, x, distributions);
                
        //we'll have to adapt more than one element    
        AdaptProductionOnLastElements(x, remainingLoad, distributions);
    }

    private void AdaptProductionOnLastElements(NodeWrapper<IPowerPlant> x, double remainingLoad, List<Distribution> distributions)
    {
        if (x.Parent.Element == null || remainingLoad >= x.Element.MinimumProduction) 
            return;
        
        //we can remove expected load from parent
        if (remainingLoad + (x.Parent.Element.Production - x.Parent.Element.MinimumProduction) > x.Element.MinimumProduction)
        {
            x.Parent.Element.Production -= x.Element.MinimumProduction - remainingLoad;
            x.Element.Production = x.Element.MinimumProduction;
                        
            StoreBranchIfBetterCost(x,distributions);
        }
    }

    private void AdaptLastElementProduction(double remainingLoad, NodeWrapper<IPowerPlant> x, List<Distribution> distributions)
    {
        if (remainingLoad <= x.Element.MaximumProduction && remainingLoad >= x.Element.MinimumProduction)
        {
            x.Element.Production =  remainingLoad;
            x.Element.IsValidDistribution = true;
            
            StoreBranchIfBetterCost(x, distributions);
        }
        
    }

    private void StoreBranchIfBetterCost(NodeWrapper<IPowerPlant> x, List<Distribution> distributions)
    {
        var branchCost = x.GetTotalCost(0);
        if (branchCost > _bestCost)
            return;
        
        distributions.Clear();
        distributions.AddRange(x.GetBranch()
            .Select(item => new Distribution(powerPlantName: item.Name, production: item.Production))
            .Reverse());
        _bestCost = branchCost;

    }

    private static bool IsFinalInDistribution(NodeWrapper<IPowerPlant> x)
    {
        return x != null && x.Element.IsValidDistribution;
    }



    private void AppendUnusedPowerPlant(List<Distribution> distributions)
    {
        distributions
            .AddRange(_availablePowerPlants
                .OrderBy(x => x.Cost)
                .Where(item => distributions.All(x => x.PowerPlantName != item.Name))
                .Select(item => new Distribution(powerPlantName: item.Name, production: 0)));
    }
}