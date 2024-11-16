using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Search;
using UnityEngine;

public class RevenueManager : MonoBehaviour
{
    public bool isLog;
    public PolicyManager policyManager;
    public TimeManager timeManager;
    private int revenue;
    private List<SpendingDelegate> spendings = new List<SpendingDelegate>();
    public RevRecorder settledRecorder = new RevRecorder();
    public RevRecorder gainRecorder = new RevRecorder();
    public RevRecorder costRecorder = new RevRecorder();

    public void Gain(int amount){

        if (isLog)
        {
            Debug.Log(revenue + "+" + amount + "=" + (revenue + amount));
        }
        gainRecorder.attachChange(amount);
        settledRecorder.attachChange(+amount);
        revenue += amount;
    
    }
    public void Cost(int amount){
        if (isLog)
        {
            Debug.Log(revenue + "-" + amount + "=" + (revenue - amount));
        }
        costRecorder.attachChange(amount);
        settledRecorder.attachChange(-amount);

        revenue -= amount;}
    public int GetRevenue()
    {
        return revenue;
    }

    public void RemoveSpendingItem(SpendingDelegate target)
    {
        Debug.Assert(spendings.Contains(target));
        spendings.Remove(target);
    }
    public void AddSpendingItem(SpendingDelegate target)
    {
        Debug.Assert(!spendings.Contains(target));
        spendings.Add(target);
    }


    void Start()
    {
        timeManager.OnHourChanged += (object sender,
        TimeManager.OnHourChangedEventArgs eventArgs) =>
        {
            for (int i = 0; i < spendings.Count; i++)
            {
                this.Cost(spendings[i].hourlyCost);
            }
            CumalativeInfoCalculation_HourlyCalled();
        };

        timeManager.OnDayChanged += (object sender,
        TimeManager.OnDayChangedEventArgs eventArgs) =>
        {
            // manager daily spendings

        };
        PlacePeopleManager.OnBuildConfirm += (object sender, PlacePeopleManager.OnBuildConfrimEventArg eventArgs) =>
        {

            this.AddSpendingItem(policyManager.GetSpendingPrototypesAccordingToPolicy(eventArgs.newPlaceBehavior.placeType));
            
        };
    }

    private void CumalativeInfoCalculation_HourlyCalled()
    {
        settledRecorder.step();
        gainRecorder.step();
        costRecorder.step();
    }
}

