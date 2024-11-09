
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

    public class OnShiftHourChangedEventArgs : EventArgs
    {
        public int newSubHour;
    }
    public event EventHandler<OnShiftHourChangedEventArgs> OnShiftHourChanged;

    private int currentDay = 1;
    private int currentHour = 0;
    public float hourDuration = 1.0f;
    //private float movingSpeed = 3.0f;
    private float timer = 0.0f;
    private float timerDaywise = 0.0f;
    private float timerSub = 0.0f;
    private int speedIndex = 0;

    private float timeScaleBeforPause = 0;
    public enum Speed
    {
        Default,
        Double,
        Triple,
        //Quad,

    }
    private List<Speed> speeds = new List<Speed> { Speed.Default, Speed.Double, Speed.Triple };
    public float GetTime24()
    {
        float scaler = 1f / hourDuration;
        return timerDaywise * scaler;
    }


    public float GetDefaultMovingSpeed()
    {
        return 5f;
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
            Time.timeScale += 0.8f;
        }
    }
    public void PauseOrResume()
    {
        if(Time.timeScale != 0)
        {
            timeScaleBeforPause = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = timeScaleBeforPause;
        }
        
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
            Time.timeScale -= 0.8f;
        }
    }


    void Update()
    {
        timerDaywise += Time.deltaTime;
        
        timer += Time.deltaTime;
        timerSub += Time.deltaTime;
        if (timerSub > hourDuration *0.6f)
        {
            timerSub = 0;
            OnShiftHourChanged?.Invoke(this, new OnShiftHourChangedEventArgs { newSubHour = currentHour });
        }

        if (timer >= hourDuration)
        {
            timer = 0.0f;
            currentHour++;
            if (currentHour < 24)
            {
                OnHourChanged?.Invoke(this, new OnHourChangedEventArgs { newHour = currentHour });
                //Debug.Log(currentDay + "  " + currentHour);
            }
            if (currentHour >= 24)
            {
                timerDaywise = 0;
                currentHour = 0;
                currentDay++;
                OnDayChanged?.Invoke(this, new OnDayChangedEventArgs { newDay = currentDay });
            }
        }
    }
}
