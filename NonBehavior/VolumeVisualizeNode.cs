using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEngine;


public class VolumeVisualizeNode
{
    //refs 
    TimeManager timeManager;
    //public static GameObject VirusVolumeTilePrefab;
    private GameObject virusVolumeTile;
    GridValuesContainer<VolumeVisualizeNode> virusVolumeGridValuesManager;

    //cell related
    Vector3Int cellPosition;

    // this value should between 0 - 100 integer
    public int virusVolume = 0;
    // this value should between 0 - 1 float
    float selfCleaningRate = 0.8f;

    public VolumeVisualizeNode(GridValuesContainer<VolumeVisualizeNode> virusVolumeGridValuesManager, Vector3Int cellPosition, TimeManager timeManager)
    {
        this.virusVolumeGridValuesManager = virusVolumeGridValuesManager;
        this.cellPosition = cellPosition;
        this.timeManager = timeManager;


        timeManager.OnHourChanged += (object sender, TimeManager.OnHourChangedEventArgs eventArgs) =>
        {
            SelfCleaning();
        };

    }
    public void SetVirusVolumeTile(GameObject virusVolumeTile)
    {
        this.virusVolumeTile = virusVolumeTile;
        this.virusVolumeTile.transform.position = this.cellPosition + new Vector3(0.5f, 0.5f);
    }

    private void SelfCleaning()
    {
        virusVolumeTile.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, virusVolume / 150f);

        if (virusVolume == 0)
        {
            return;
        }
        virusVolume = (int)(virusVolume * selfCleaningRate);
        virusVolumeGridValuesManager.TriggerGridObjectChanged(cellPosition);
    }
    public void SetSelfCleaningSpeed(float newSelfCleaningRate)
    {
        selfCleaningRate = newSelfCleaningRate;
    }
    public void SetVirusVolume(int newVolume)
    {
        virusVolume = math.max(virusVolume, newVolume);
        virusVolumeGridValuesManager.TriggerGridObjectChanged(cellPosition);
    }
    public override string ToString()
    {
        return virusVolume.ToString();
    }

}