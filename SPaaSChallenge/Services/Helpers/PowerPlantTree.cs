using SPaaSChallenge.Models;

namespace SPaaSChallenge.Services.Helpers;


public class PowerPlantTree<T>(T element)
    where T : IPowerPlant
{
    public readonly T Element = element;

    public static implicit operator PowerPlantTree<T>(T t) => new (t);

    public PowerPlantTree<T> Parent;  
    
    public void BuildTree(List<T> powerPlants, Action<PowerPlantTree<T>> computeProduction, Func<PowerPlantTree<T>, bool> isFinalNode) 
    {
        AddChildren(powerPlants, this, computeProduction, isFinalNode);
    }
    
    private static void AddChildren(List<T> powerPlants, PowerPlantTree<T> parent, Action<PowerPlantTree<T>> computeProduction, Func<PowerPlantTree<T>, bool> isFinalNode)
    {
        var remainingPowerPlants = powerPlants.ToList();
        
        foreach (PowerPlantTree<T> node in powerPlants)
        {
            node.Parent = parent;

            computeProduction.Invoke(node);
            
            if (isFinalNode.Invoke(node)) 
                continue;

            remainingPowerPlants.RemoveAt(0);
            AddChildren(remainingPowerPlants, node, computeProduction,isFinalNode);
        }
        
    }
    
    public double GetParentsLoad()
    {
        double totalLoad = 0;

        var currentElement =Parent;
        while (currentElement.Element != null)
        {
            totalLoad += currentElement.Element.Production;
            currentElement = currentElement.Parent;
        }

        return totalLoad;
    }
    
    
    public double GetTotalCost(double totalCost)
    {
        var currentElement = this;

        while (Parent != null && currentElement.Element != null)
        {
            totalCost += currentElement.Element.Production * currentElement.Element.Cost;
            currentElement = currentElement.Parent;
        }
        return totalCost;
    }

    public List<T> GetBranch(List<T> list = null)
    {
        list ??= [];
        
        if (Element != null)
        {
            list.Add(Element);
        }

        Parent?.GetBranch(list);

        return list;
    }


}