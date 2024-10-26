using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridValuesAttachedBehavior : MonoBehaviour
{
    public Grid grid;
    public TimeManager timeManager;
    public int width, height;
    public GameObject volumeVisualizeTilePrefab;
    public bool isVisualizePathFinding;
    public bool isVisualizeVolume;



    public GridValuesContainer<PathFindingNode> pathFindingGVC;
    public GridValuesContainer<VolumeVisualizeNode> volumeGVC;

    void Awake()
    {
        pathFindingGVC = new GridValuesContainer<PathFindingNode>(
            isVisualizePathFinding,
            timeManager,
            grid,
            width,
            height,
            (GridValuesContainer<PathFindingNode> g, Vector3Int v, TimeManager tm) => new PathFindingNode(g, v, tm));
        volumeGVC = new GridValuesContainer<VolumeVisualizeNode>(
            isVisualizeVolume,
            timeManager,
            grid,
            width,
            height,
            (GridValuesContainer<VolumeVisualizeNode> g, Vector3Int v, TimeManager tm) => new VolumeVisualizeNode(g, v, tm));

        // set volume visualize tile for virusVolumeGridValuesManager
        for (int i = 0; i < volumeGVC.width; i++)
        {
            for (int j = 0; j < volumeGVC.height; j++)
            {
                GameObject newVirusTile = Instantiate(volumeVisualizeTilePrefab);
                volumeGVC.GetGridObj(new UnityEngine.Vector3(i, j)).SetVirusVolumeTile(newVirusTile);
                GameObject newPopuTile = Instantiate(volumeVisualizeTilePrefab);
                volumeGVC.GetGridObj(new UnityEngine.Vector3(i, j)).SetPopuVolumeTile(newPopuTile);
            }
        }
    }

    public UnityEngine.Vector3 GetWorldGridCenter()
    {
        UnityEngine.Vector3 localCenter =  new UnityEngine.Vector3((width / 2) * grid.cellSize.x, (height / 2) * grid.cellSize.y, -10);
        return transform.TransformPoint(localCenter);
    }

    void Update()
    {
        
    }
}
