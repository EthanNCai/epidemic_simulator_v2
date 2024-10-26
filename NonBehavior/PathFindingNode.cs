using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PathFindingNode
{
    private GridValuesContainer<PathFindingNode> gridValuesManager;
    public int value;
    public int gCost, hCost, fCost;
    public PathFindingNode cameFrom;
    public Vector3Int cellPosition;
    public bool isWalkable = true;
    //public bool manSetWalkableFlag = false;
    public bool defactRoutePoint = false;

    public PathFindingNode(GridValuesContainer<PathFindingNode> gridValuesManager, Vector3Int cellPosition, TimeManager timeManager)
    {
        this.gridValuesManager = gridValuesManager;
        this.cellPosition = cellPosition;
        this.gCost = int.MaxValue;
        this.CalculateFCost();
        this.cameFrom = null;
        //this.isPlaceUpHead = false;
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
    public void SetFalseWalkable()
    {
        isWalkable = false;
        gridValuesManager.TriggerGridObjectChanged(cellPosition);
    }
    public void SetTrueWalkable()
    {
        isWalkable = true;
        gridValuesManager.TriggerGridObjectChanged(cellPosition);
    }

    public void addValue()
    {
        this.value += 1;
        gridValuesManager.TriggerGridObjectChanged(cellPosition);
    }

    public void CalculateFCost()
    {
        int temp = defactRoutePoint ? 2 * (gCost + hCost ) : gCost + hCost;
        
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
