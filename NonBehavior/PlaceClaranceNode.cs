using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Timeline;

public class PlaceClaranceNode
{
    private static readonly object gridValuesContainerLock = new object();

    TimeManager timeManager;
    GridValuesContainer<PlaceClaranceNode> placeClaranceGVC;
    bool isBuilt = false;
    public int[] availableClaranceNEWS = new int[4];
    const int NORTH = 0;
    const int SOUTH = 3;
    const int EAST = 1;
    const int WEST = 2;

    Vector3Int cellPosition;
    public PlaceClaranceNode(GridValuesContainer<PlaceClaranceNode> placeClaranceGVC, Vector3Int cellPosition, TimeManager timeManager)
    {
        this.placeClaranceGVC = placeClaranceGVC;
        this.cellPosition = cellPosition;
        this.timeManager = timeManager;
    }
    
    public void SetBuilt(bool target)
    {
        isBuilt = target;
    }
    string GetArrString(int[] nice)
    {
        string output = "";
        for (int i = 0; i < nice.Length; i++)
        {
            output += nice[i].ToString() + ",";
        }
        return output;
    }
    async public Task CalculateClarance()
    {
        PlaceClaranceNode[,] gridValue = placeClaranceGVC.GetGridValuesArray();
        int grid_width = placeClaranceGVC.width;
        int grid_height = placeClaranceGVC.height;
        // top
        this.availableClaranceNEWS = new int[4];

        List<Task> tasks = new List<Task>();

        tasks.Add(Task.Run(() =>
        {
            int temp_x = cellPosition.x - 1;
            int temp_y = cellPosition.y;

            while (temp_x >= 0)
            {
                lock (gridValuesContainerLock)
                {
                    if (gridValue[temp_x, temp_y].isBuilt == false)
                    {
                        temp_x--;
                        
                            availableClaranceNEWS[WEST] += 1;
                        
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }));
        tasks.Add(Task.Run(() => {
            int temp_x = cellPosition.x + 1;
            int temp_y = cellPosition.y;

            while (temp_x < grid_width)
            {
                lock (gridValuesContainerLock)
                {
                    if (gridValue[temp_x, temp_y].isBuilt == false)
                    {
                        temp_x++;
                        availableClaranceNEWS[EAST] += 1;
                    }
                    else
                    {
                        break;
                    }
                    
                }
            }
        }));
        tasks.Add(Task.Run(() => {
            int temp_x = cellPosition.x;
            int temp_y = cellPosition.y - 1;
            while (temp_y >= 0)
            {
                lock (gridValuesContainerLock)
                {
                    if (gridValue[temp_x, temp_y].isBuilt == false)
                    {
                        temp_y--;
                        
                         availableClaranceNEWS[SOUTH] += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }));
        tasks.Add(Task.Run(() => {
            int temp_x = cellPosition.x;
            int temp_y = cellPosition.y + 1;
            while (temp_y < grid_height)
            {
                lock (gridValuesContainerLock)
                {
                    if (gridValue[temp_x, temp_y].isBuilt == false)
                    {
                        temp_y++;
                        availableClaranceNEWS[NORTH] += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }));
        await Task.WhenAll(tasks);
        placeClaranceGVC.TriggerGridObjectChanged(cellPosition);
    }


    public override string ToString()
    {
        return availableClaranceNEWS[NORTH].ToString() + " " + availableClaranceNEWS[EAST].ToString() + "\n" +
            availableClaranceNEWS[WEST].ToString() + " " + availableClaranceNEWS[SOUTH].ToString() + "\n";
    }

    //public override string ToString()
    //{
    //    return cellPosition.x.ToString() + "," + cellPosition.y.ToString();
    //}

    public (int, int) GetNorthSouth()
    {
        return (availableClaranceNEWS[NORTH], availableClaranceNEWS[SOUTH]);
    }
    public (int, int) GetWestEast()
    {
        return (availableClaranceNEWS[WEST], availableClaranceNEWS[EAST]);
    }
}
