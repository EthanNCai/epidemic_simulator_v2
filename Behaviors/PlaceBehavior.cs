using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlaceBehavior : MonoBehaviour
{
    public UnityEngine.Vector3 cellPosition;
    public Vector3 size;
    private System.Random randomGenerator = new System.Random();
    GridValuesContainer<PathFindingNode> pathFindingNodeValuesManager;
    public PlaceType placeType;
    public SocialStatus vSocialStatusDst;
    

    public void init(PlacePrototype placePrototype,GridValuesAttachedBehavior gridValuesAttachedBehavior)
    {
        this.placeType = placePrototype.placeType;
        this.vSocialStatusDst = PlacePrototype.VSocialStatusDstInferFromPlaceType(placeType);
        this.cellPosition = placePrototype.cellPosition;
        this.size = placePrototype.size;
        //this.height = placePrototype.height;
        transform.localScale = size;
        transform.position = cellPosition;
        pathFindingNodeValuesManager = gridValuesAttachedBehavior.pathFindingGVC;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (j == i && i != size.x-1) { continue; }
                pathFindingNodeValuesManager.GetGridObj(new Vector3(cellPosition.x + i, cellPosition.y + j)).ToggleWalkable();
            }
        }

    }

    public void shallowInit(PlaceType placeType)
    {
        Vector3 newSize = PlacePrototype.GetPlaceSize(placeType);
        transform.localScale = newSize;
    }


    public Vector3 GenerateInPlacePosition()
    {
        return new Vector3(
        cellPosition.x + (float)(size.x * randomGenerator.NextDouble()),
        cellPosition.y + (float)(size.y * randomGenerator.NextDouble()));
    }
    
}
