using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidHighVolumeManager : MonoBehaviour
{
    //CONSTS

    private int HIGH_VOLUME_GATE = 50;
    private int HIGH_POPU_GATE = 5;

    public GridValuesAttachedBehavior gridValuesAttachedBehavior;
    public TimeManager timeManager;
    private GridValuesContainer<PathFindingNode> pathFindingGVC;
    private GridValuesContainer<VolumeVisualizeNode> volumeGVC;
    void Start()
    {
        pathFindingGVC = gridValuesAttachedBehavior.pathFindingGVC;
        volumeGVC = 
            gridValuesAttachedBehavior.volumeGVC;
        timeManager.OnHourChanged += (object sender, TimeManager.OnHourChangedEventArgs eventArgs) =>
        {
            // 遍历所有的pathFidingNodes
            for (int i = 0; i < pathFindingGVC.width; i++)
            {
                for (int j = 0; j < pathFindingGVC.height; j++)
                {
                    PathFindingNode temp_pfn = pathFindingGVC.GetGridObj(new Vector3(i,  j));
                    VolumeVisualizeNode temp_vvn = volumeGVC.GetGridObj(new Vector3(i, j));

                    temp_pfn.defactRoutePoint = 
                    temp_vvn.virusVolume > HIGH_VOLUME_GATE || 
                    temp_vvn.popuVolume > HIGH_POPU_GATE ? true : false;
                    
                }
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
