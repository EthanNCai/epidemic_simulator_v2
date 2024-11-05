using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;



//  *** RESPONSIBILITIES *** 
// A -> Generate Preview Tile and handles its life during BUILDING PROCESS
// B -> Hold a Container for PLACES and PERSONS
//  ************************
public class PlacePeopleManager : MonoBehaviour
{
    //VARIABLES ->  static infos
    private List<PlacePrototype> homes_debug =
        new List<PlacePrototype> {
            //new Place(new UnityEngine.Vector3(2, 2),1,1),
            new PlacePrototype(new UnityEngine.Vector3(10, 10),PlaceType.Home,new Vector3(2,2)),
            new PlacePrototype(new UnityEngine.Vector3(4, 10),PlaceType.Home,new Vector3(2,2)),
            new PlacePrototype(new UnityEngine.Vector3(11, 5),PlaceType.Home,new Vector3(2,2)),
        };
    private List<PlacePrototype> offices_debug =
        new List<PlacePrototype> {
            new PlacePrototype(new UnityEngine.Vector3(12, 3),PlaceType.Office,new Vector3(2,2)),
            new PlacePrototype(new UnityEngine.Vector3(20, 10),PlaceType.Office,new Vector3(3,3)),
            new PlacePrototype(new UnityEngine.Vector3(6, 6),PlaceType.Office,new Vector3(2,2)),
            //new Place(new Vector3(35, 4),2,2),
        };
    // ***********************

    // VARIABLES ->  refs
    public TimeManager timeManager;
    // ***********************

    // VARIABLES -> place person container - related
    public RevenueManager revenueManager;
    public GridValuesAttachedBehavior gridValuesAttachedBehavior;
    public PeopleInfectionManager personInfectionManager;
    public GameObject personPrefab;
    public GameObject placePrefab;
    private List<PlaceBehavior> homes = new List<PlaceBehavior>();
    private List<PlaceBehavior> offices = new List<PlaceBehavior>();
    public List<GameObject> persons = new List<GameObject>();
    // ***********************

    // VARIABLES -> building process - related
    GameObject placePreviewingObj = null;
    // ***********************

    void Start()
    {
        // generate the people
        for (int i = 0; i < offices_debug.Count; i++)
        {
            GameObject newPlace = Instantiate(placePrefab);
            PlaceBehavior newBehavior = newPlace.GetComponent<PlaceBehavior>();
            newBehavior.init(
                offices_debug[i],
                gridValuesAttachedBehavior
                );
            offices.Add(newBehavior);
        }
        for (int j = 0; j < homes_debug.Count; j++)
        {
            GameObject newPlace = Instantiate(placePrefab);
            PlaceBehavior newBehavior = newPlace.GetComponent<PlaceBehavior>();
            newBehavior.init(
                homes_debug[j],
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
                        null,
                        revenueManager
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
            new Infection(),
            revenueManager
            );
        persons.Add(newPerson_);
        personInfectionManager.someOneJustGotInfected(newPerson_);


        //Event subscribetion -> building related

        BuidableBehavior.OnBuildingPreviewStart += (object sender,
        BuidableBehavior.OnBuildingPreviewStartEvnentArg eventArgs) =>
        {
            GeneratePlacePreview(eventArgs.placeType);
        };
        // ***********************
    }

    public int GetPeopleCounts()
    {
        return persons.Count;
    }

    // Funcs -> building-related 
    public void GeneratePlacePreview(PlaceType placeType)
    {
        if (placePreviewingObj != null)
        {
            Debug.LogError("ni xian kai de");
            Destroy(placePreviewingObj);
        }
        placePreviewingObj = Instantiate(placePrefab);
        placePreviewingObj.GetComponent<PlaceBehavior>().shallowInit(placeType);
    }
    public void DestroyPlacePreview()
    {
        Assert.IsTrue(placePreviewingObj != null );
        //placePreviewingObj = Instantiate(placePrefab);

    }
    public void ConfirmPlacePreview()
    {
        Assert.IsTrue(placePreviewingObj != null);
        //GameObject tempPlaceObj = Instantiate(placePrefab);

    }
    // ***********************



    void Update()
    {
        if (placePreviewingObj != null)
        {
            placePreviewingObj.transform.position = Input.mousePosition;
        }
        //Debug.Log(Input.mousePosition.ToString());
    }

}
