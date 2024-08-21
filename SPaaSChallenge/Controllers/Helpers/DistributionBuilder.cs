using SPaaSChallenge.Models;
using SPaaSChallenge.Services;

namespace SPaaSChallenge.Controllers.Helpers;


public class DistributionBuilder : IDistributionBuilder
{
    private double _load;
    private IEnumerable<IPowerPlant> _availablePowerPlants;
    

    public DistributionBuilder SetLoad(double load)
    {
        this._load = load;
        return this;
    }
    
    public DistributionBuilder SetPowerPlants( IEnumerable<IPowerPlant> powerPlants)
    {
        _availablePowerPlants = powerPlants;
        return this;
    }
    
    private void AppendUnusedPowerPlant(List<Distribution> distributions)
    {
        distributions
            .AddRange(_availablePowerPlants
                .OrderBy(x => x.Cost)
                .Where(item => distributions.All(x => x.PowerPlantName != item.Name))
                .Select(item => new Distribution(powerPlantName: item.Name, production : 0)));
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
        List<Distribution> distributions = [];
        var powerPlants = _availablePowerPlants.OrderBy(x => x.Cost).ToList();

        while (_load > 0)
        {
            var currentPowerPlant = powerPlants.FirstOrDefault();
            if (currentPowerPlant == null)
            {
                return distributions;
            }
            
            powerPlants.Remove(currentPowerPlant);

            //powerPlant doesn't meet the minimal requirement
            if (currentPowerPlant.MinimumProduction > _load)
            {
                continue;
            }
            
            //we'll need an extra powerPlant to meet the load requirement
            if (_load > currentPowerPlant.MaximumProduction)
            {
                //if there is no possibility to meet the load requirement with this powerPlant we try with the next one
                var isThereMatchingPowerPlantToCompleteLoad = TryGetNextMatchingPowerPlant(out IPowerPlant nextEligiblePowerPlant,powerPlants ,currentPowerPlant);
                if (!isThereMatchingPowerPlantToCompleteLoad)
                {
                    continue;
                }
                
                var removableAmount = ComputeMaximumRemovableAmountBasedOnNextPowerPlant(nextEligiblePowerPlant, currentPowerPlant);
                distributions.Add(new Distribution (powerPlantName : currentPowerPlant.Name, production : removableAmount));
                _load -= removableAmount;
            }

            //this powerPlant is enough to cover the load requirement
            else
            {
                distributions.Add(new Distribution (powerPlantName : currentPowerPlant.Name, production : _load ));
                _load = 0.0;
            }
            
        }

        AppendUnusedPowerPlant(distributions);
        return distributions;
    }

    private bool TryGetNextMatchingPowerPlant(out IPowerPlant nextEligiblePowerPlant, List<IPowerPlant> powerPlants, IPowerPlant currentPowerPlant)
    {
        nextEligiblePowerPlant = powerPlants.FirstOrDefault(x => (x.MinimumProduction + currentPowerPlant.MinimumProduction) <= _load);
        return nextEligiblePowerPlant != null;
    }


    private double ComputeMaximumRemovableAmountBasedOnNextPowerPlant(IPowerPlant nextEligiblePowerPlant, IPowerPlant currentPowerPlant)
    {
        var removableAmount = _load - nextEligiblePowerPlant.MinimumProduction;

        if (removableAmount > currentPowerPlant.MaximumProduction)
        {
            removableAmount = currentPowerPlant.MaximumProduction;
        }

        return removableAmount;
    }
}

