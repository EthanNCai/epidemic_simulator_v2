using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleManager : MonoBehaviour
{
    public GridValuesAttachedBehavior gridValuesAttachedBehavior;
    public PeopleInfectionManager personInfectionManager;
    public GameObject personPerfab;
    public TimeManager timeManager;
    private List<Place> homes =
        new List<Place> {
            //new Place(new UnityEngine.Vector3(2, 2),1,1),
            new Place(new UnityEngine.Vector3(10, 10),2,2),
            new Place(new UnityEngine.Vector3(4, 10),2,2),
            new Place(new UnityEngine.Vector3(11, 5),2,2),
        };
    private List<Place> offices =
        new List<Place> {
            new Place(new UnityEngine.Vector3(12, 3),2,2),
            new Place(new UnityEngine.Vector3(12, 10),2,2),
            new Place(new UnityEngine.Vector3(6, 6),2,2),
            //new Place(new Vector3(35, 4),2,2),
        };
    public List<GameObject> persons = new List<GameObject>();
    void Start()
    {
        // generate the people
        for (int i = 0; i < offices.Count; i++)
        {
            for (int j = 0; j < homes.Count; j++)
            {
                for (int k = 0; k < 100; k++)
                {
                    GameObject newPerson = Instantiate(personPerfab);
                    newPerson.GetComponent<PersonBehavior>().init(
                        homes[i],
                        offices[i],
                        gridValuesAttachedBehavior,
                        timeManager,
                        personInfectionManager,
                        newPerson,
                        null
                        );
                    persons.Add(newPerson);
                }
            }
        }
        GameObject newPerson_ = Instantiate(personPerfab);
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
