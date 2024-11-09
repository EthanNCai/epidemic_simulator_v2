using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEngine;


public class VolumeVisualizeNode
{
    // GENERAL 
    private TimeManager timeManager;
    private GridValuesContainer<VolumeVisualizeNode> volumeGVC;
    Vector3Int cellPosition;

    //VIRUS VOLUME PART

    public int virusVolume = 0;     // this value should between 0 - 100 integer
    float VIRUS_SELF_CLEAN_RATE = 0.95f; // this value should between 0 - 1 float
    private GameObject virusVisualizeTile;


    // POPULARITY VOLUME PART
    public int popuVolume = 0;     // this value should between 0 - 100 integer
    float POPU_SELF_CLEAN_RATE = 0.80f; // this value should between 0 - 1 float
    private GameObject popuVisualizeTile;


    //cell related

    public VolumeVisualizeNode(GridValuesContainer<VolumeVisualizeNode> volumeGVC, Vector3Int cellPosition, TimeManager timeManager)
    {
        this.volumeGVC = volumeGVC;
        this.cellPosition = cellPosition;
        this.timeManager = timeManager;


        timeManager.OnHourChanged += (object sender, TimeManager.OnHourChangedEventArgs eventArgs) =>
        {
            popuVolumeSelfCleaning();
            virusVolumeSelfCleaning();
        };

    }
    public void SetVirusVolumeTile(GameObject virusVisualizeTile)
    {
        this.virusVisualizeTile = virusVisualizeTile;
        this.virusVisualizeTile.transform.position = this.cellPosition + new UnityEngine.Vector3(0.5f, 0.5f);
    }
    public void SetPopuVolumeTile(GameObject popuVisualizeTile)
    {
        this.popuVisualizeTile = popuVisualizeTile;
        this.popuVisualizeTile.transform.position = this.cellPosition + new UnityEngine.Vector3(0.5f, 0.5f);
    }
    public void SetVirusVolume(int newVolume)
    {
        virusVolume = math.max(virusVolume, newVolume);
        volumeGVC.TriggerGridObjectChanged(cellPosition);
    }
    public void SetPopuVolume(int newVolume)
    {
        popuVolume = newVolume;
        volumeGVC.TriggerGridObjectChanged(cellPosition);
    }
    private void virusVolumeSelfCleaning()
    {
        // update color visualization
        virusVisualizeTile.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, virusVolume / 150f);

        if (virusVolume == 0){return;} // cleared
        
        virusVolume = (int)(virusVolume * VIRUS_SELF_CLEAN_RATE);
        volumeGVC.TriggerGridObjectChanged(cellPosition);
    }
    //private void 
    private void popuVolumeSelfCleaning()
    {
        // update color visualization
        popuVisualizeTile.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, math.min(popuVolume / 10f,1f));

        if (popuVolume == 0) { return; } // cleared

        popuVolume = (int)(popuVolume * POPU_SELF_CLEAN_RATE);
        volumeGVC.TriggerGridObjectChanged(cellPosition);
    }

    
    public override string ToString()
    {
        return virusVolume.ToString();
    }

}