using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RevRecorder
{
    public int hourly = 0;
    public int daily = 0;
    public int dailyTrend = 0;
    public int hourlyTrend = 0;

    //temp
    public int oldValue;
    public Queue<int> dailyQueue = new Queue<int>();
    public int hourTemp = 0;
    public void attachChange(int change)
    {
        hourTemp += change;
    }
    public void step()
    {
        // you shall hourly call this...
        // the parameter we need is actually how much we earned in last hour.


        // maintain the queue
        dailyQueue.Enqueue(hourTemp);
        if (dailyQueue.Count > 24) { dailyQueue.Dequeue(); }


        // calculate the hourly stuff
        int newHourly = hourTemp;
        hourlyTrend = hourly != 0 ? (newHourly / hourly) * 100 : 100;
        hourly = newHourly;
        hourTemp = 0;


        // calcaulate the daily stuff
        int newDaily = dailyQueue.Sum();
        dailyTrend = daily != 0 ? (newDaily / daily) * 100 : 100;
        daily = newDaily;

    }
}
