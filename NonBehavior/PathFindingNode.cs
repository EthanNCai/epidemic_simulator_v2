using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PathFindingNode
{
    private GridValuesContainer<PathFindingNode> gridValuesManager;
    public int gCost, hCost, fCost;
    public PathFindingNode cameFrom;
    public Vector3Int cellPosition;
    public bool isDefactNode;
    public bool isWalkable = true;


    public PathFindingNode(GridValuesContainer<PathFindingNode> gridValuesManager, Vector3Int cellPosition, TimeManager timeManager)
    {
        this.gridValuesManager = gridValuesManager;
        this.cellPosition = cellPosition;
        this.gCost = int.MaxValue;
        this.CalculateFCost();
        this.cameFrom = null;
    }
    public void ResetSelf()
    {
        gCost = int.MaxValue;
        CalculateFCost();
        cameFrom = null;
    }

    public void ToggleWalkable()
    {
        isWalkable = !isWalkable;
        gridValuesManager.TriggerGridObjectChanged(cellPosition);
    }
    public void SetWalkable(bool value)
    {
        isWalkable=value;
        gridValuesManager.TriggerGridObjectChanged(cellPosition);
    }

    public void CalculateFCost()
    {

        fCost = isDefactNode? 2 * (gCost + hCost) : gCost + hCost;
    }
    public override string ToString()
    {
        //return cellPosition.x.ToString() +',' + cellPosition.y.ToString();
        return isWalkable.ToString() + '\n' + cellPosition.x.ToString() + ',' + cellPosition.y.ToString();
        //return "";
    }

    public override int GetHashCode()
    {
        return cellPosition.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is PathFindingNode other)
        {
            return cellPosition == other.cellPosition;
        }
        return false;
    }

}
