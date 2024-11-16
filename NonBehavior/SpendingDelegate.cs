using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpendingDelegate 
{
    public string name;
    public int hourlyCost;
    //public int dailyCost;
    public int personwiseCost;

    public SpendingDelegate(string name, int hourlyCost, int personwiseCost)
    {
        this.name = name;
        this.hourlyCost = hourlyCost;
        this.personwiseCost = personwiseCost;
    }
}
