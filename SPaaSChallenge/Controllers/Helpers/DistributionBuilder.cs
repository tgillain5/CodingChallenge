using SPaaSChallenge.Models;
using SPaaSChallenge.Services;

namespace SPaaSChallenge.Controllers.Helpers;

public class DistributionBuilder(ILogger<DistributionBuilder> logger) : IDistributionBuilder
{
    private double _load;
    private IEnumerable<IPowerPlant> _availablePowerPlants;
    
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

        while (_load > 0)
        {
            var currentPowerPlant = powerPlants.FirstOrDefault();
            if (currentPowerPlant == null)
            {
                return distributions;
            }

            powerPlants.Remove(currentPowerPlant);

            if (currentPowerPlant.MinimumProduction > _load)
            {
                logger.LogInformation("powerPlant: {Name} doesn't meet the minimal requirement we try with the next one", currentPowerPlant.Name);
                continue;
            }
            
            logger.LogInformation("powerPlant: {Name} is not enough to cover the load requirement we'll need an extra powerPlant to meet the load requirement", currentPowerPlant.Name);
            if (_load > currentPowerPlant.MaximumProduction)
            {
                var isThereMatchingPowerPlantToCompleteLoad = TryGetNextMatchingPowerPlant(out var nextEligiblePowerPlant, powerPlants, currentPowerPlant);
                logger.LogInformation("powerPlant: {Name} will be use to complete the distribution", nextEligiblePowerPlant.Name);
                if (!isThereMatchingPowerPlantToCompleteLoad)
                {
                    continue;
                }

                var removableAmount = ComputeMaximumRemovableAmountBasedOnNextPowerPlant(nextEligiblePowerPlant, currentPowerPlant);
                distributions.Add(new Distribution(powerPlantName: currentPowerPlant.Name, production: removableAmount));
                _load -= removableAmount;
            }
            else
            {
                logger.LogInformation("powerPlant: {Name} is enough to cover the load requirement and finish the distribution", currentPowerPlant.Name);
                distributions.Add(new Distribution(powerPlantName: currentPowerPlant.Name, production: _load));
                _load = 0.0;
            }
        }

        AppendUnusedPowerPlant(distributions);
        return distributions;
    }

    private bool TryGetNextMatchingPowerPlant(out IPowerPlant nextEligiblePowerPlant, List<IPowerPlant> powerPlants,
        IPowerPlant currentPowerPlant)
    {
        nextEligiblePowerPlant = powerPlants.FirstOrDefault(x => (x.MinimumProduction + currentPowerPlant.MinimumProduction) <= _load);
        return nextEligiblePowerPlant != null;
    }

    private void AppendUnusedPowerPlant(List<Distribution> distributions)
    {
        distributions
            .AddRange(_availablePowerPlants
                .OrderBy(x => x.Cost)
                .Where(item => distributions.All(x => x.PowerPlantName != item.Name))
                .Select(item => new Distribution(powerPlantName: item.Name, production: 0)));
    }

    private double ComputeMaximumRemovableAmountBasedOnNextPowerPlant(IPowerPlant nextEligiblePowerPlant,
        IPowerPlant currentPowerPlant)
    {
        var removableAmount = _load - nextEligiblePowerPlant.MinimumProduction;

        if (removableAmount > currentPowerPlant.MaximumProduction)
        {
            removableAmount = currentPowerPlant.MaximumProduction;
        }

        return removableAmount;
    }
}