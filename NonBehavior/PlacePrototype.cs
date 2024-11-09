using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public enum PlaceType
{
    Office,
    Home,
    Clinic,
    Hospital,
    TestCentre,
    InsolationShip
}
public class PlacePrototype
{
    public  UnityEngine.Vector3 cellPosition;
    public Vector3 size;
    private System.Random randomGenerator = new System.Random();
    public PlaceType placeType;
    public SocialStatus VirtualSocialStatusDst;

    

    public PlacePrototype(UnityEngine.Vector3 cellPosition, PlaceType placeType, Vector3 size)
    {
        this.placeType = placeType;
        //if(Mathf.Min(height, width ) < 2)
        //{
        //    Debug.LogError("Place Size Should no smaller than 2 Units");
        //}
        this.cellPosition = cellPosition;
        this.size = size;
        //this.width = width;
        //this.height = height;
    }

    public Vector3 GenerateInPlacePosition()
    {
        return new Vector3(
        cellPosition.x + (float)(size.x * randomGenerator.NextDouble()),
        cellPosition.y + (float)(size.y * randomGenerator.NextDouble()));
    }

    static public SocialStatus VSocialStatusDstInferFromPlaceType(PlaceType target)
    {
        switch (target)
        {
            case PlaceType.Home: { return SocialStatus.Homing; }
            case PlaceType.Office: { return SocialStatus.Working; }
            case PlaceType.Clinic: {
                    return SocialStatus.InClinic;
                }
            default: { return SocialStatus.Moving; }
        }
    }

    static public string[] PlaceTypeTexts = new string[] { "家", "公司","诊所","医院" };
    static public string GetPlaceTypeDescriber(PlaceType target)
    {
        switch (target)
        {
            case PlaceType.Home: { 
                    return PlaceTypeTexts[0]; }
            case PlaceType.Office: { 
                    return PlaceTypeTexts[1]; }
            case PlaceType.Clinic:
                {
                    return PlaceTypeTexts[2];
                }
            case PlaceType.Hospital:
                {
                    return PlaceTypeTexts[3];
                }
            default: { return "Something bad happens...."; }
        }
    }
    static public Vector3 GetPlaceSize(PlaceType target)
    {
        switch (target)
        {
            case PlaceType.Home:
                {
                    return new Vector3(2,2);
                }
            case PlaceType.Office:
                {
                    return new Vector3(3,3);
                }
            case PlaceType.Clinic:
                {
                    return new Vector3(2,3);
                }
            case PlaceType.Hospital:
                {
                    return new Vector3(4, 4);
                }
            default: { return new Vector3(-1, -1); }
        }
    }
   

}
