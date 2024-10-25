using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleManager : MonoBehaviour
{
    public GridValuesAttachedBehavior gridValuesAttachedBehavior;
    public GameObject personPerfab;
    public TimeManager timeManager;
    private List<Place> homes =
        new List<Place> {
            //new Place(new UnityEngine.Vector3(2, 2),1,1),
            new Place(new UnityEngine.Vector3(10, 10),1,1),
            new Place(new UnityEngine.Vector3(13, 4),2,2),
            new Place(new UnityEngine.Vector3(11, 5),2,2),
        };
    private List<Place> offices =
        new List<Place> {
            new Place(new UnityEngine.Vector3(12, 7),1,1),
            new Place(new UnityEngine.Vector3(12, 10),1,1),
            new Place(new UnityEngine.Vector3(6, 6),2,2),
            //new Place(new Vector3(35, 4),2,2),
        };
    void Start()
    {
        for (int i = 0; i < offices.Count; i++)
        {
            for (int j = 0; j < homes.Count; j++)
            {
                for (int k = 0; k < 100; k++)
                {
                    GameObject newPerson = Instantiate(personPerfab);
                    newPerson.GetComponent<PersonBehavior>().init(homes[i], offices[i], gridValuesAttachedBehavior,timeManager);
                }

            }
        }
    }


    void Update()
    {
        
    }
}
