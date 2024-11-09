using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevenueManager : MonoBehaviour
{
    // Start is called before the first frame update

    private int revenue;

    public void PayTaxes(int amount)
    {
        revenue += amount;
    }
    public void Cost(int amount)
    {
        revenue -= amount;
    }
    public int GetRevenue()
    {
        return revenue;
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
