using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Linq;
using Unity.VisualScripting;
using System;

public class GridValuesContainer<TGridObject>
{

    public int width, height;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugGridArray;
    public Grid grid;
    public TimeManager timeManager;

    public class OnGridValueChangedEventArgs : EventArgs
    {
        public Vector3Int position;
    }
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    HashSet<Vector2Int> validCells = new HashSet<Vector2Int>();
    public TGridObject[,] GetGridArray()
    {
        return gridArray;
    }
    public GridValuesContainer(bool showDebugText, TimeManager timeManager, Grid grid, int width, int height, Func<GridValuesContainer<TGridObject>, Vector3Int, TimeManager, TGridObject> createGridObj)
    {
        if (grid.cellSize.x != grid.cellSize.y) { Debug.LogError("grid cell must be a square"); }

        this.timeManager = timeManager;
        this.grid = grid;
        this.width = width;
        this.height = height;

        gridArray = new TGridObject[width, height];
        debugGridArray = new TextMesh[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y);
                gridArray[x, y] = createGridObj(this, cellPosition, timeManager);
            }
        }

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                if (showDebugText)
                {
                    Vector3 TextPosition = grid.CellToLocal(new Vector3Int(x, y)) + new Vector3(grid.cellSize.x, grid.cellSize.y) * 0.5f;
                    Vector3 TextWorldPosition = grid.transform.TransformPoint(TextPosition);
                    debugGridArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, TextWorldPosition, 35, Color.white, TextAnchor.MiddleCenter);

                }
                validCells.Add(new Vector2Int(x, y));
            }
        }
        if (showDebugText)
        {
            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
            {
                // modify texts on the grid
                TextMesh textMesh = debugGridArray[eventArgs.position.x, eventArgs.position.y];
                textMesh.text = gridArray[eventArgs.position.x, eventArgs.position.y].ToString();
            };
        }

    }
    


    public TGridObject GetGridObj(Vector3 localPosition)
    {
        //Vector3Int cellPosition = grid.LocalToCell(localPosition);

        Vector3Int cellPosition =  new Vector3Int((int)localPosition.x, (int)localPosition.y);
        if (CheckPositionValid(cellPosition))
        {

            return gridArray[cellPosition.x, cellPosition.y];
        }
        else
        {
            return default;
        }
    }
    public bool CheckPositionValid(Vector3Int target)
    {
        return validCells.Contains((Vector2Int)target);
    }
    public void TriggerGridObjectChanged(Vector3Int cellPosition)
    {
        OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { position = cellPosition });
    }


}