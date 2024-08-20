using SPaaSChallenge.Services;

namespace SPaaSChallenge.Models;

public class ElectricalNetwork
{
    private readonly double _load;
    private readonly IEnumerable<IPowerPlant> _availablePowerPlants;
    public List<Distribution> Distributions = [];


    public ElectricalNetwork(double load, IEnumerable<IPowerPlant> powerPlants)
    {
        _availablePowerPlants = powerPlants;
        _load = load;
        ValidateDistributionPossibility();

        SetDistribution();
        AppendUnusedPowerPlant();
    }

    private void AppendUnusedPowerPlant()
    {
        Distributions
            .AddRange(_availablePowerPlants
                .OrderBy(x => x.Cost)
                .Where(item => Distributions.All(x => x.PowerPlantName != item.Name))
                .Select(item => new Distribution(powerPlantName: item.Name, production : 0)));
    }


    private void ValidateDistributionPossibility()
    {
        if (!_availablePowerPlants.Any(x => x.MinimumProduction < _load))
        {
            throw new DistributionImpossibleException("Impossible to provide the expected load");   
        }
        
        if (_availablePowerPlants.Sum(x => x.MaximumProduction) < _load)
        {
            throw new DistributionImpossibleException("Impossible to provide the expected load");
        }
       
    }

    private void SetDistribution()
    {
        var tempLoad = _load;
        Distributions = [];
        var powerPlants = _availablePowerPlants.OrderBy(x => x.Cost).ToList();

        while (tempLoad > 0)
        {
            var currentPowerPlant = powerPlants.FirstOrDefault();
            if (currentPowerPlant == null)
            {
                return;
            }
            
            powerPlants.Remove(currentPowerPlant);

            //powerPlant doesn't meet the minimal requirement
            if (currentPowerPlant.MinimumProduction > tempLoad)
            {
                continue;
            }
            
            //we'll need an extra powerPlant to meet the load requirement
            if (tempLoad > currentPowerPlant.MaximumProduction)
            {
                //if there is no possibility to meet the load requirement with this powerPlant we try with the next one
                var nextEligiblePowerPlant = powerPlants.FirstOrDefault(x => (x.MinimumProduction + currentPowerPlant.MinimumProduction) <= tempLoad);
                if (nextEligiblePowerPlant == null)
                {
                    continue;
                }
                
                var removableAmount = tempLoad - nextEligiblePowerPlant.MinimumProduction;

                if (removableAmount > currentPowerPlant.MaximumProduction)
                {
                    removableAmount = currentPowerPlant.MaximumProduction;
                }
                
                Distributions.Add(new Distribution (powerPlantName : currentPowerPlant.Name, production : removableAmount));
                tempLoad -= removableAmount;
            }

            //this powerPlant is enough to cover the load requirement
            else
            {
                Distributions.Add(new Distribution (powerPlantName : currentPowerPlant.Name, production : tempLoad ));
                tempLoad = 0.0;
            }
            
        }

    }
    
}

