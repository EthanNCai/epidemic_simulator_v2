using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum InfectionStatus
{
    Period1,
    Period2,
    Recovered,
    UnInfected,
}


public class Infection
{
    // between 0 - 100
    const int MAX_VIRUS_VOLUME = 100;
    const int MIN_VIRUS_VOLUME = 0;
    private int virusVolume = 0;
    private int period1;
    private int period2;
    private int period1DaysLeft;
    private int period2DaysLeft;

    private int MapValue(int value, int fromMin, int fromMax, int toMin, int toMax)
    {
        return (int)((value - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin);
    }
    public Infection(int maxPeriod1 = 14, int maxPeriod2 = 7)
    {
        this.period1 = Random.Range(2, maxPeriod1);
        this.period1DaysLeft = period1;
        this.period2 = Random.Range(2, maxPeriod2);
        this.period2DaysLeft = period2;
    }

    public void Progress()
    {

        if (period1DaysLeft > 0)
        {

            virusVolume = MapValue(period1 - period1DaysLeft, 0, period1, MIN_VIRUS_VOLUME, MAX_VIRUS_VOLUME);
            period1DaysLeft -= 1;
        }
        else if (period2DaysLeft > 0)
        {

            virusVolume = MapValue(period2 - period2DaysLeft, 0, period2, MAX_VIRUS_VOLUME, MIN_VIRUS_VOLUME);
            period2DaysLeft -= 1;
        }

    }

    public InfectionStatus CheckStatus()
    {
        if (period1DaysLeft > 0)
        {
            return InfectionStatus.Period1;
        }
        else if (period2DaysLeft > 0)
        {
            return InfectionStatus.Period2;
        }
        else
        {
            return InfectionStatus.Recovered;
        }
    }

    public int CheckVirusVolume()
    {
        return virusVolume;

    }
    public int ChekcPeriodDaysLeft()
    {

        if (CheckStatus() == InfectionStatus.Period1)
        {
            return period1DaysLeft;
        }
        else if (CheckStatus() == InfectionStatus.Period2)
        {
            return period2DaysLeft;
        }
        else
        {
            return -1;
        }
    }
}
