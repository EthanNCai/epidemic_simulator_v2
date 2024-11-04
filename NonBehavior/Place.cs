using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public enum PlaceType
{
    Office,
    Home,
    Hospital,
    TestCentre,
    InsolationShip
}
public class Place
{
    public  UnityEngine.Vector3 cellPosition;
    public int width, height;
    private System.Random randomGenerator = new System.Random();
    public PlaceType placeType;
    public SocialStatus VirtualSocialStatusDst;

    

    public Place(UnityEngine.Vector3 cellPosition, PlaceType placeType, int width, int height)
    {
        this.placeType = placeType;
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

    static public SocialStatus VSocialStatusDstInferFromPlaceType(PlaceType target)
    {
        switch (target)
        {
            case PlaceType.Home: { return SocialStatus.Homing; }
            case PlaceType.Office: { return SocialStatus.Working; }
            default: { return SocialStatus.Moving; }
        }
    }

    static public string[] PlaceTypeTexts = new string[] { "家", "公司" };
    static public string GetPlaceTypeDescriber(PlaceType target)
    {
        switch (target)
        {
            case PlaceType.Home: { 
                    return PlaceTypeTexts[0]; }
            case PlaceType.Office: { 
                    return PlaceTypeTexts[1]; }
            default: { return "不是..哥们"; }
        }
    }

}
