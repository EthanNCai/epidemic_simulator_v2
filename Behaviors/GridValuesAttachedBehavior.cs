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
    public bool isVisualizeClarance;
    public GridValuesContainer<PathFindingNode> pathFindingGVC;
    public GridValuesContainer<VolumeVisualizeNode> virusVolumeGVC;
    public GridValuesContainer<PlaceClaranceNode> placeClaranceGVC;


    void Start()

    {
        pathFindingGVC = new GridValuesContainer<PathFindingNode>(
            isVisualizePathFinding,
            timeManager,
            grid,
            width,
            height,
            (GridValuesContainer<PathFindingNode> g, Vector3Int v, TimeManager tm) => new PathFindingNode(g, v, tm));

        virusVolumeGVC = new GridValuesContainer<VolumeVisualizeNode>(

            isVisualizeVolume,
            timeManager,
            grid,
            width,
            height,
            (GridValuesContainer<VolumeVisualizeNode> g, Vector3Int v, TimeManager tm) => new VolumeVisualizeNode(g, v, tm));

        placeClaranceGVC = new GridValuesContainer<PlaceClaranceNode>(
            isVisualizeClarance,
            timeManager,
            grid,
            width,
            height,
            (GridValuesContainer<PlaceClaranceNode> g, Vector3Int v, TimeManager tm) => new PlaceClaranceNode(g, v, tm));

        // set volume visualize tile for virusVolumeGridValuesManager

        for (int i = 0; i < virusVolumeGVC.width; i++)
        {
            for (int j = 0; j < virusVolumeGVC.height; j++)
            {
                GameObject newTile = Instantiate(virusVolumeTilePrefab);
                virusVolumeGVC.GetGridObj(new UnityEngine.Vector3(i, j)).SetVirusVolumeTile(newTile);

            }
        }
    }

    public UnityEngine.Vector3 GetWorldGridCenter()
    {
        UnityEngine.Vector3 localCenter =  new UnityEngine.Vector3((width / 2) * grid.cellSize.x, (height / 2) * grid.cellSize.y, -10);
        return transform.TransformPoint(localCenter);
    }
    public UnityEngine.Vector3 GetWorldTopRightCorner()
    {
        Vector3 localCenter = new Vector3(width * grid.cellSize.x, height * grid.cellSize.y, -10);
        return transform.TransformPoint(localCenter);
    }
    public UnityEngine.Vector3 GetWorldBottomLeftCorner()
    {
        Vector3 localCenter = new Vector3(0,0, -10);
        return transform.TransformPoint(localCenter);
    }
    void Update()
    {
        
    }
}
