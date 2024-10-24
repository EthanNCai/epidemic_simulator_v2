
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class PathFinding
{
    private static Dictionary<PathFindingNode, HashSet<PathFindingNode>> neiborDict = new Dictionary<PathFindingNode, HashSet<PathFindingNode>>();
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private GridValuesContainer<PathFindingNode> pathFindingGridValuesManager;
    private SortedSet<PathFindingNode> openList = new SortedSet<PathFindingNode>(new FScoreComparer());
    private HashSet<PathFindingNode> closeList = new HashSet<PathFindingNode>();
    private PathFindingNode[] arrayRefFromGrid;
    



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

    public void FindPath(PathFindingNode startNode, PathFindingNode endNode, Stack<Vector3> pathStack)
    {
            pathStack.Clear();
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
                PathFindingNode currentNode = openList.First();
                //PathFindingNode currentNode = GetLowestFCostNode(openList);
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

    private void CalculatePathStack(PathFindingNode endNode, Stack<Vector3> pathStack)
    {
        pathStack.Push(endNode.cellPosition);
        PathFindingNode currentNode = endNode;
        while (currentNode.cameFrom != null)
        {
            pathStack.Push(currentNode.cameFrom.cellPosition);
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
    private PathFindingNode GetLowestFCostNode(List<PathFindingNode> targetList)
    {
        targetList.Sort((node1, node2) => node1.fCost.CompareTo(node2.fCost));
        return targetList[0];
    }
    private class FScoreComparer : IComparer<PathFindingNode>
    {
        public int Compare(PathFindingNode x, PathFindingNode y)
        {
            return x.fCost.CompareTo(y.fCost);
        }
    }

}