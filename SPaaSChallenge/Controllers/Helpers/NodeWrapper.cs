using SPaaSChallenge.Models;

namespace SPaaSChallenge.Controllers.Helpers;

public class NodeWrapper<T>(T element)
    where T : IPowerPlant
{
    public readonly T Element = element;
    public static implicit operator NodeWrapper<T>(T t) => new (t);

    public NodeWrapper<T> Parent;  
    
    public void AddChildren(List<T> powerPlants,NodeWrapper<T> parent, Action<NodeWrapper<T>> computeProduction , Func<NodeWrapper<T>,bool> isFinalNode)
    {
        var remainingPowerPlants = powerPlants.ToList();
        
        foreach (NodeWrapper<T> node in powerPlants)
        {
            node.Parent = parent;

            computeProduction.Invoke(node);
            
            if (isFinalNode.Invoke(node)) 
                continue;

            remainingPowerPlants.RemoveAt(0);
            node.AddChildren(remainingPowerPlants,node, computeProduction,isFinalNode);
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