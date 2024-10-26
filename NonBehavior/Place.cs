using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place
{
    public  UnityEngine.Vector3 cellPosition;
    public int width, height;
    private System.Random randomGenerator = new System.Random();


    public Place(UnityEngine.Vector3 cellPosition, int width, int height)
    {
        if(Mathf.Min(height, width ) < 2)
        {
            Debug.LogError("Place Size Should no smaller than 2 Units");
        }
        this.cellPosition = cellPosition;
        this.width = width;
        this.height = height;
    }

    public Vector3 GenerateInPlacePosition()
    {
        return new Vector3(
        cellPosition.x + (float)(width * randomGenerator.NextDouble()),
        cellPosition.y + (float)(height * randomGenerator.NextDouble()));
    }

    
}
