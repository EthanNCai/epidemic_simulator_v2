
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;


public class PathFinding
{

    private const int PATH_JITTER_RATIO = 1;


    private static Dictionary<PathFindingNode, HashSet<PathFindingNode>> neiborDict = new Dictionary<PathFindingNode, HashSet<PathFindingNode>>();
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private GridValuesContainer<PathFindingNode> pathFindingGridValuesManager;
    //private SortedSet<PathFindingNode> openList = new SortedSet<PathFindingNode>(new FScoreComparer());
    private List<PathFindingNode> openList = new List<PathFindingNode>();
    private HashSet<PathFindingNode> closeList = new HashSet<PathFindingNode>();
    private PathFindingNode[] arrayRefFromGrid;
    System.Random randomGenerator = new System.Random();

    public PathFinding(GridValuesContainer<PathFindingNode> gridValuesContainer)
    {

        this.pathFindingGridValuesManager = gridValuesContainer;
        PathFindingNode[,] arrayRefFromGrid_ = pathFindingGridValuesManager.GetGridArray();
        this.arrayRefFromGrid = arrayRefFromGrid_.Cast<PathFindingNode>().ToArray();

        if (neiborDict.Count == 0)
        {
            CalculateNeiborDict();
        }
    }

    public void FindPath(Vector3 startLocalPos, Vector3 endLocalPos, Stack<UnityEngine.Vector3> pathStack)
    {

            PathFindingNode startNode = pathFindingGridValuesManager.GetGridObj(startLocalPos);
            PathFindingNode endNode = pathFindingGridValuesManager.GetGridObj(endLocalPos);
            if(startNode == endNode) { return; }    

            openList.Clear();
            openList.Add(startNode);
            closeList.Clear();
            for (int i = 0; i < arrayRefFromGrid.Length; i++)
            {
                arrayRefFromGrid[i].ResetSelf();
            }


            // Initialize the start node
            startNode.gCost = 0;
            startNode.hCost = CalculateDistance(startNode, endNode);
            startNode.CalculateFCost();
            while (openList.Count > 0)
            {
                //PathFindingNode currentNode = openList.First();
                PathFindingNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
                {
                    CalculatePathStack(endNode, pathStack);
                    return;

                }
                openList.Remove(currentNode);
                closeList.Add(currentNode);
                if (!currentNode.isWalkable)
                {
                    continue;
                }
                foreach (PathFindingNode neiborNode in GetNeiborNodes(currentNode))
                {
                    int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neiborNode);
                    if (tentativeGCost < neiborNode.gCost)
                    {
                        neiborNode.cameFrom = currentNode;
                        neiborNode.gCost = tentativeGCost;
                        neiborNode.hCost = CalculateDistance(neiborNode, endNode);
                        neiborNode.CalculateFCost();
                        if (!openList.Contains(neiborNode))
                        {
                            openList.Add(neiborNode);
                        }
                    }
                }
            }
        Debug.LogWarning("No path Found");
        return;
        
    }
    private void CalculateNeiborDict()
    {
        
        neiborDict.Clear();
        for (int i = 0; i < arrayRefFromGrid.Length; i++)
        {
            neiborDict.Add(arrayRefFromGrid[i], CalculateNodeNeibors(arrayRefFromGrid[i]));

        }
        
    }

    private HashSet<PathFindingNode> GetNeiborNodes(PathFindingNode currentNode)
    {
        return new HashSet<PathFindingNode>(neiborDict[currentNode].Except(closeList));
    }
    private HashSet<PathFindingNode> CalculateNodeNeibors(PathFindingNode currentNode)
    {
        HashSet<PathFindingNode> neiborNodes = new HashSet<PathFindingNode>();
        for (int a = -1; a <= 1; a++)
        {
            for (int b = -1; b <= 1; b++)
            {
                if (a != 0 || b != 0)
                {

                    Vector3Int neiborCellPosition = currentNode.cellPosition + new Vector3Int(a, b);
                    PathFindingNode neiborNode = pathFindingGridValuesManager.GetGridObj(neiborCellPosition);
                    if (pathFindingGridValuesManager.CheckPositionValid(neiborCellPosition))
                    {
                        neiborNodes.Add(neiborNode);
                    }

                }
            }
        }
        return neiborNodes;

    }

    private void CalculatePathStack(PathFindingNode endNode, Stack<UnityEngine.Vector3> pathStack)
    {
        pathStack.Push(endNode.cellPosition);
        PathFindingNode currentNode = endNode;
        while (currentNode.cameFrom != null)
        {
            pathStack.Push(RadiusJitter(currentNode.cameFrom.cellPosition, PATH_JITTER_RATIO));
            //pathStack.Push(currentNode.cameFrom.cellPosition);
            currentNode = currentNode.cameFrom;
        }
        return;
    }

    private int CalculateDistance(PathFindingNode a, PathFindingNode b)
    {
        int xDistance = Mathf.Abs(a.cellPosition.x - b.cellPosition.x);
        int yDistance = Mathf.Abs(a.cellPosition.y - b.cellPosition.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private class FScoreComparer : IComparer<PathFindingNode>
    {
        public int Compare(PathFindingNode x, PathFindingNode y)
        {
            return x.fCost.CompareTo(y.fCost);
        }
    }

    public Vector3 RadiusJitter(Vector3 cellPos, int radius)
    {
        cellPos.x += (float)(PATH_JITTER_RATIO * randomGenerator.NextDouble());
        cellPos.y += (float)(PATH_JITTER_RATIO *randomGenerator.NextDouble());
        return cellPos;
    }


    private List<PathFindingNode> OldGetNeiborNodes(PathFindingNode currentNode)
    {
        List<PathFindingNode> neibors = new List<PathFindingNode>();
        // eight different direction
        for (int a = -1; a <= 1; a++)
        {
            for (int b = -1; b <= 1; b++)
            {
                if (a != 0 || b != 0)
                {

                    Vector3Int neiborCellPosition = currentNode.cellPosition + new Vector3Int(a, b);
                    PathFindingNode neiborNode = pathFindingGridValuesManager.GetGridObj(neiborCellPosition);
                    if (closeList.Contains(neiborNode))
                    {
                        continue;
                    }
                    if (pathFindingGridValuesManager.CheckPositionValid(neiborCellPosition))
                    {
                        neibors.Add(neiborNode);
                    }

                }
            }
        }
        return neibors;

    }
    private PathFindingNode GetLowestFCostNode(List<PathFindingNode> targetList)
    {
        targetList.Sort((node1, node2) => node1.fCost.CompareTo(node2.fCost));
        return targetList[0];
    }
}