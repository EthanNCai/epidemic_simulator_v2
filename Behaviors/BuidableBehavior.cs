using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static TimeManager;

public class BuidableBehavior:MonoBehaviour 
{
    PlaceType placeType;

    public static event EventHandler<OnBuildingPreviewStartEvnentArg> OnBuildingPreviewStart;
    public class OnBuildingPreviewStartEvnentArg : EventArgs
    {
        public PlaceType placeType;
    }
    //public event EventHandler<OnHourChangedEventArgs> OnHourChanged;

    public void init(PlaceType placeType)
    {
        this.placeType = placeType;
        SetBuidableDisplay();
    }
    private void SetBuidableDisplay()
    {
       transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = PlacePrototype.GetPlaceTypeDescriber(this.placeType);
    }
    public void StartBuilding()
    {
        OnBuildingPreviewStart?.Invoke(this, new OnBuildingPreviewStartEvnentArg { placeType = this.placeType });
        //Ini
    }

}
