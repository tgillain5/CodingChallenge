using SPaaSChallenge.Models;

namespace SPaaSChallenge.Services.Helpers;

public class PowerPlantTree<T>(T element) where T : IPowerPlant
{
    public readonly T Element = element;

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
        
            if (node.Element.IsValidDistribution) 
                continue;
            
            remainingPowerPlants.RemoveAt(0);
            AddChildren(remainingPowerPlants, node, computeProduction);
        }
        
    }
    
    public double GetParentsLoad()
    {
        double totalLoad = 0;

        var currentElement = Parent;
        while (currentElement.Element != null)
        {
            totalLoad += currentElement.Element.Production;
            currentElement = currentElement.Parent;
        }

        return totalLoad;
    }
    
    
    public double GetTotalCost(double totalCost = 0)
    {
        var currentElement = this;

        while (Parent != null && currentElement.Element != null)
        {
            totalCost += currentElement.Element.Production * currentElement.Element.Cost;
            currentElement = currentElement.Parent;
        }
        return totalCost;
    }

    public IList<T> GetBranch(List<T> list = null)
    {
        list ??= [];
        
        if (Element != null)
            list.Add(Element);
        
        Parent?.GetBranch(list);
        return list;
    }


}