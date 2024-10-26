using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public enum PlaceType
{
    Home,
    Office,
    Hosipital,
    TestField,
    IsolationField,
}


public class PlaceBehavior : MonoBehaviour
{
    public Color homeColor;
    public Color officeColor;
    public UnityEngine.Vector3 cellPosition;
    public int width, height;
    private System.Random randomGenerator = new System.Random();
    GridValuesContainer<PathFindingNode> pathFindingNodeValuesManager;
    PlaceType placeType;
    public void init(Vector3 cellPosition, int width, int height, GridValuesAttachedBehavior gridValuesAttachedBehavior, PlaceType placeType)
    {
        this.placeType = placeType;
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
                
                PathFindingNode temp = pathFindingNodeValuesManager.GetGridObj(new Vector3(cellPosition.x + i, cellPosition.y + j));

                //temp.manSetWalkableFlag = true;
                if (j == 0 && i == 0) { continue; }
                temp.SetFalseWalkable();
                
            }
        }
        Color targetColor = Color.white;
        switch (placeType)
        {
            case PlaceType.Office: { targetColor = officeColor; break; }
            case PlaceType.Home: { targetColor = homeColor; break; }
        }
        Transform childTransform = transform.GetChild(0);
        childTransform.GetComponent<SpriteRenderer>().color = targetColor;

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
