using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.PlayerLoop;

public class PlaceBehavior : MonoBehaviour
{
    static int SIZE_TO_CAPACITY_RATIO = 5;


    public UnityEngine.Vector3 cellPosition;
    public Vector3 size;
    private System.Random randomGenerator = new System.Random();
    GridValuesContainer<PathFindingNode> pathFindingGVC;
    public GridValuesContainer<PlaceClaranceNode> placeClaranceGVC;
    public PlaceType placeType;
    public SocialStatus vSocialStatusDst;
    
    // calculate capacity
    public int capacity = -1;
    public List<PersonBehavior> personsInHere = new List<PersonBehavior>();
    public List<PersonBehavior> personsAppointedHere = new List<PersonBehavior>();

    public void init(PlacePrototype placePrototype,GridValuesAttachedBehavior gridValuesAttacher)
    {
        //Debug.Log(capacity);
        this.placeType = placePrototype.placeType;
        this.vSocialStatusDst = PlacePrototype.VSocialStatusDstInferFromPlaceType(placeType);
        this.cellPosition = placePrototype.cellPosition;
        this.size = placePrototype.size;
        //this.height = placePrototype.height;
        transform.localScale = size;
        transform.position = cellPosition;
        pathFindingGVC = gridValuesAttacher.pathFindingGVC;
        placeClaranceGVC = gridValuesAttacher.placeClaranceGVC;
        capacity = (int)(size.x * size.y) * SIZE_TO_CAPACITY_RATIO;
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                // set built flag for clarance calculation
                placeClaranceGVC.GetGridObj(new Vector3(cellPosition.x + i, cellPosition.y + j)).SetBuilt(true);

                // set walkable flag for pathfinding
                if (!(j == i && i < size.x-1)) 
                {
                    pathFindingGVC.GetGridObj(new Vector3(cellPosition.x + i, cellPosition.y + j)).SetWalkable(false);
                }

                
            }
        }

    }

    public void shallowInit(PlaceType placeType)
    {
        Vector3 newSize = PlacePrototype.GetPlaceSize(placeType);
        transform.localScale = newSize;
    }


    public Vector3 GenerateInPlacePosition()
    {
        return new Vector3(
        cellPosition.x + (float)(size.x * randomGenerator.NextDouble()),
        cellPosition.y + (float)(size.y * randomGenerator.NextDouble()));
    }
    public void personEntered(PersonBehavior person)
    {
        // everyPersonMustBe Appointed before entered
        Assert.IsTrue(personsAppointedHere.Contains(person));
        personsAppointedHere.Remove(person);
        personsInHere.Add(person);
    }
    public void personAppointed(PersonBehavior person)
    {
        personsAppointedHere.Add(person);
    }
    public void personExited(PersonBehavior person)
    {
        personsInHere.Remove(person);
    }
    public bool CheckIsAvaliable()
    {
        return (personsAppointedHere.Count + personsInHere.Count) < capacity;
    }

    
}
