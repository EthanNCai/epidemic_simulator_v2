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
    private int period1InTotal;
    private int period2InTotal;
    private int period1DaysLeft;
    private int period2DaysLeft;

    static public string[] InfectionStatusTexts = new string[] { "潜伏期", "发病期", "未感染过" , "已痊愈" };
    static public string GetInfectionStatusDescriber(InfectionStatus target)
    {
        switch (target)
        {
            case InfectionStatus.Period1: { return InfectionStatusTexts[0]; }
            case InfectionStatus.Period2: { return InfectionStatusTexts[1]; }
            case InfectionStatus.UnInfected: { return InfectionStatusTexts[2]; }
            case InfectionStatus.Recovered: { return InfectionStatusTexts[3]; }
            default: { return "错误"; }
        }
    }

    private int MapValue(int value, int fromMin, int fromMax, int toMin, int toMax)
    {
        return (int)((value - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin);
    }
    public Infection(int maxPeriod1 = 14, int maxPeriod2 = 7)
    {
        this.period1InTotal = Random.Range(2, maxPeriod1);
        this.period1DaysLeft = period1InTotal;
        this.period2InTotal = Random.Range(2, maxPeriod2);
        this.period2DaysLeft = period2InTotal;
    }

    public bool ProgressAndReturnIsNextStage()
    {
        if (period1DaysLeft > 0)
        {
            virusVolume = MapValue(period1InTotal - period1DaysLeft, 0, period1InTotal, MIN_VIRUS_VOLUME, MAX_VIRUS_VOLUME);
            period1DaysLeft -= 1;
        }
        else if (period2DaysLeft > 0)
        {
            virusVolume = MapValue(period2InTotal - period2DaysLeft, 0, period2InTotal, MAX_VIRUS_VOLUME, MIN_VIRUS_VOLUME);
            period2DaysLeft -= 1;
        }
        // stage change observation
        bool isStageChanged = period1DaysLeft == 0 && period2DaysLeft == period2InTotal;
        return isStageChanged;
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
