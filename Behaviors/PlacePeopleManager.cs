using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePeopleManager : MonoBehaviour
{
    public GridValuesAttachedBehavior gridValuesAttachedBehavior;
    public PeopleInfectionManager personInfectionManager;
    public GameObject placePrefab;
    public GameObject personPrefab;
    //public Camera mainCamera;
    
    public TimeManager timeManager;
    private List<Place> homes_debug =
        new List<Place> {
            //new Place(new UnityEngine.Vector3(2, 2),1,1),
            new Place(new UnityEngine.Vector3(10, 10),2,2),
            new Place(new UnityEngine.Vector3(4, 10),2,2),
            new Place(new UnityEngine.Vector3(11, 5),2,2),
        };
    private List<Place> offices_debug =
        new List<Place> {
            new Place(new UnityEngine.Vector3(12, 3),2,2),
            new Place(new UnityEngine.Vector3(20, 10),3,3),
            new Place(new UnityEngine.Vector3(6, 6),2,2),
            //new Place(new Vector3(35, 4),2,2),
        };
    private List<PlaceBehavior> homes = new List<PlaceBehavior>();
    private List<PlaceBehavior> offices = new List<PlaceBehavior>();
    public List<GameObject> persons = new List<GameObject>();
    void Start()
    {
        // generate the people
        for (int i = 0; i < offices_debug.Count; i++)
        {
            GameObject newPlace = Instantiate(placePrefab);
            PlaceBehavior newBehavior = newPlace.GetComponent<PlaceBehavior>();
            newBehavior.init(
                offices_debug[i].cellPosition,
                offices_debug[i].width,
                offices_debug[i].height,
                gridValuesAttachedBehavior
                );
            offices.Add(newBehavior);
        }
        for (int j = 0; j < homes_debug.Count; j++)
        {
            GameObject newPlace = Instantiate(placePrefab);
            PlaceBehavior newBehavior = newPlace.GetComponent<PlaceBehavior>();
            newBehavior.init(
                homes_debug[j].cellPosition,
                homes_debug[j].width,
                homes_debug[j].height,
                gridValuesAttachedBehavior
                );
            homes.Add(newBehavior);
        }


        for (int i = 0; i < offices_debug.Count; i++)
        {
            for (int j = 0; j < homes_debug.Count; j++)
            {
                for (int k = 0; k < 100; k++)
                {
                    GameObject newPerson = Instantiate(personPrefab);
                    newPerson.GetComponent<PersonBehavior>().init(
                        homes[i],
                        offices[i],
                        gridValuesAttachedBehavior,
                        timeManager,
                        personInfectionManager,
                        newPerson,
                        null
                        );
                    //newPerson.GetComponent<HoverColorChanger>().init(
                    //    Camera.main
                    //    );
                    persons.Add(newPerson);
                }
            }
        }
        GameObject newPerson_ = Instantiate(personPrefab);
        newPerson_.GetComponent<PersonBehavior>().init(
            homes[0],
            offices[0],
            gridValuesAttachedBehavior,
            timeManager,
            personInfectionManager,
            newPerson_,
            new Infection()
            );
        persons.Add(newPerson_);
        personInfectionManager.someOneJustGotInfected(newPerson_);
    }


    void Update()
    {
        
    }
}
