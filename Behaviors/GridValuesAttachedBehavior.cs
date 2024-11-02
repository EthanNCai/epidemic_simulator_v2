using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridValuesAttachedBehavior : MonoBehaviour
{
    public Grid grid;
    public TimeManager timeManager;
    public int width, height;
    public GameObject virusVolumeTilePrefab;
    public bool isVisualizePathFinding;
    public bool isVisualizeVolume;



    public GridValuesContainer<PathFindingNode> pathFindingGVC;
    public GridValuesContainer<VolumeVisualizeNode> virusVolumeGridValuesManager;

    void Awake()
    {
        pathFindingGVC = new GridValuesContainer<PathFindingNode>(
            isVisualizePathFinding,
            timeManager,
            grid,
            width,
            height,
            (GridValuesContainer<PathFindingNode> g, Vector3Int v, TimeManager tm) => new PathFindingNode(g, v, tm));
        virusVolumeGridValuesManager = new GridValuesContainer<VolumeVisualizeNode>(
            isVisualizeVolume,
            timeManager,
            grid,
            width,
            height,
            (GridValuesContainer<VolumeVisualizeNode> g, Vector3Int v, TimeManager tm) => new VolumeVisualizeNode(g, v, tm));

        // set volume visualize tile for virusVolumeGridValuesManager
        for (int i = 0; i < virusVolumeGridValuesManager.width; i++)
        {
            for (int j = 0; j < virusVolumeGridValuesManager.height; j++)
            {
                GameObject newTile = Instantiate(virusVolumeTilePrefab);
                virusVolumeGridValuesManager.GetGridObj(new UnityEngine.Vector3(i, j)).SetVirusVolumeTile(newTile);
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
