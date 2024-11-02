using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlaceBehavior : MonoBehaviour
{
    public UnityEngine.Vector3 cellPosition;
    public int width, height;
    private System.Random randomGenerator = new System.Random();
    GridValuesContainer<PathFindingNode> pathFindingNodeValuesManager;

    public void init(Vector3 cellPosition, int width, int height, GridValuesAttachedBehavior gridValuesAttachedBehavior)
    {

        this.cellPosition = cellPosition;
        this.width = width;
        this.height = height;
        transform.localScale = new Vector3(width,height);
        transform.position = cellPosition;
        pathFindingNodeValuesManager = gridValuesAttachedBehavior.pathFindingGVC;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (j == i && i != width-1) { continue; }
                pathFindingNodeValuesManager.GetGridObj(new Vector3(cellPosition.x + i, cellPosition.y + j)).ToggleWalkable();
            }
        }

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public Vector3 GenerateInPlacePosition()
    {
        return new Vector3(
        cellPosition.x + (float)(width * randomGenerator.NextDouble()),
        cellPosition.y + (float)(height * randomGenerator.NextDouble()));
    }
}
