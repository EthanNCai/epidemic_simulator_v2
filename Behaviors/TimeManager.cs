
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static TimeManager;



public class TimeManager : MonoBehaviour
{
    public class OnHourChangedEventArgs : EventArgs
    {
        public int newHour;
    }
    public event EventHandler<OnHourChangedEventArgs> OnHourChanged;
    public class OnDayChangedEventArgs : EventArgs
    {
        public int newDay;
    }
    public event EventHandler<OnDayChangedEventArgs> OnDayChanged;

    private int currentDay = 1;
    private int currentHour = 0;
    public float hourDuration = 1.0f;
    private float movingSpeed = 3.0f;
    private float timer = 0.0f;
    //public Speed speed;
    private int speedIndex = 0;
    public enum Speed
    {
        Default,
        Double,
        Triple,
        Quad,

    }
    private List<Speed> speeds = new List<Speed> { Speed.Default, Speed.Double, Speed.Triple, Speed.Quad };

    private Speed GetSpeed()
    {
        return speeds[speedIndex];
    }
    public float GetMovingSpeed()
    {
        return movingSpeed;
    }

    private float GetCorrespondingHourDuration()
    {
        switch (GetSpeed())
        {
            case Speed.Default: { return 0.9f; }
            case Speed.Double: { return 0.7f; }
            case Speed.Triple: { return 0.5f; }
            case Speed.Quad: { return 0.4f; }
            //case Speed.Max: { return 0.3f; }
            default: return 1.0f;
        }
    }
    private float GetCorrespondingMovingSpeed()
    {
        switch (GetSpeed())
        {
            case Speed.Default: { return 12.0f; }
            case Speed.Double: { return 16.0f; }
            case Speed.Triple: { return 19.0f; }
            case Speed.Quad: { return 7.0f; }
            //case Speed.Max: { return 25.0f; }
            default: return 1.0f;
        }
    }


    public void SpeedUp()
    {
        if (speedIndex + 1 >= speeds.Count)
        {
            return;
        }
        else
        {
            speedIndex++;
        }
        hourDuration = GetCorrespondingHourDuration();
        movingSpeed = GetCorrespondingMovingSpeed();
    }

    public void SpeedDown()
    {

        if (speedIndex - 1 < 0)
        {
            return;
        }
        else
        {
            speedIndex--;
        }
        hourDuration = GetCorrespondingHourDuration();
        movingSpeed = GetCorrespondingMovingSpeed();
    }



    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= hourDuration)
        {
            timer = 0.0f;
            currentHour++;
            if (currentHour < 24)
            {
                OnHourChanged?.Invoke(this, new OnHourChangedEventArgs { newHour = currentHour });
                Debug.Log(currentDay + "  " + currentHour);
            }
            if (currentHour >= 24)
            {
                currentHour = 0;
                currentDay++;
                OnDayChanged?.Invoke(this, new OnDayChangedEventArgs { newDay = currentDay });
            }
        }
    }
}
