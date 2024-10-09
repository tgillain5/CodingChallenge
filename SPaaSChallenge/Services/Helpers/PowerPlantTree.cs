using SPaaSChallenge.Models;

namespace SPaaSChallenge.Services.Helpers;

public class PowerPlantTree<T>(T element): IPowerPlant where T : class, IPowerPlant
{
    private readonly T _element = element;

    public string Name => _element.Name;
    public double MinimumProduction => _element?.MinimumProduction ?? 0;
    public double MaximumProduction => _element?.MaximumProduction ?? 0;
    public double Cost => _element.Cost;

    public double Production
    {
        
        get => _element?.Production ?? 0;
        set => _element.Production = value;
    }
    public bool IsValidDistribution 
    { 
        get => _element.IsValidDistribution;
        set => _element.IsValidDistribution = value; 
    }
    
    public static implicit operator PowerPlantTree<T>(T t) => new (t);

    public PowerPlantTree<T> Parent;  
    
    /// <summary>
    /// we build the tree to compute best cost to do that the tree needs to know how to computeProduction.
    /// since it's not a tree navigation logic this is out of the PowerPlantTree class
    /// </summary>
    /// <param name="powerPlants"></param>
    /// <param name="computeProduction"></param>
    public void BuildDistribution(IEnumerable<T> powerPlants, Action<PowerPlantTree<T>> computeProduction) 
    {
        AddChildren(powerPlants.ToList(), this, computeProduction);
    }
    
    private static void AddChildren(IList<T> powerPlants, PowerPlantTree<T> parent, Action<PowerPlantTree<T>> computeProduction)
    {
        var remainingPowerPlants = powerPlants.ToList();
        
        foreach (PowerPlantTree<T> node in powerPlants)
        {
            node.Parent = parent;
            computeProduction.Invoke(node);
        
            if (node._element.IsValidDistribution) 
                continue;
            
            remainingPowerPlants.RemoveAt(0);
            AddChildren(remainingPowerPlants, node, computeProduction);
        }
        
    }
    
    public double GetParentsLoad()
    {
        double totalLoad = 0;

        var currentElement = Parent;
        while (currentElement._element != null)
        {
            totalLoad += currentElement._element.Production;
            currentElement = currentElement.Parent;
        }

        return totalLoad;
    }
    
    
    public double GetTotalCost(double totalCost = 0)
    {
        var currentElement = this;

        while (Parent != null && currentElement._element != null)
        {
            totalCost += currentElement._element.Production * currentElement._element.Cost;
            currentElement = currentElement.Parent;
        }
        return totalCost;
    }

    public IList<T> GetBranch(List<T> list = null)
    {
        list ??= [];
        
        if (_element != null)
            list.Add(_element);
        
        Parent?.GetBranch(list);
        return list;
    }
    
}