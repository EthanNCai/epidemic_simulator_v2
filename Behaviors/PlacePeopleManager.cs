using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using static BuidableBehavior;



//  *** RESPONSIBILITIES *** 
// A -> Generate Preview Tile and handles its life during BUILDING PROCESS
// B -> Hold a Container for PLACES and PERSONS
//  ************************
public class PlacePeopleManager : MonoBehaviour
{
    //VARIABLES ->  static infos
    public static event EventHandler<OnBuildConfrimEventArg> OnBuildConfirm;
    public class OnBuildConfrimEventArg : EventArgs
    {
        public List<PlaceBehavior> updatedPlaceBehaviorList;
        public PlaceBehavior newPlaceBehavior;
    }

    public Color buidableColor;
    public Color notBuidableColor;
    public ActionCancelRegister actionCancelRegister;
    private List<PlacePrototype> homes_debug =
        new List<PlacePrototype> {
            //new Place(new UnityEngine.Vectdor3(2, 2),1,1),
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

    private List<PlacePrototype> clinic_debug =
        new List<PlacePrototype> {
            new PlacePrototype(new UnityEngine.Vector3(14, 14),PlaceType.Clinic,new Vector3(1,1)),
            new PlacePrototype(new UnityEngine.Vector3(10, 13),PlaceType.Clinic,new Vector3(1,2))
        };
    // ***********************

    // VARIABLES ->  refs
    public TimeManager timeManager;
    // ***********************

    // VARIABLES -> place person container - related
    public RevenueManager revenueManager;
    public GridValuesAttachedBehavior gridValuesAttacher;
    public PeopleInfectionManager personInfectionManager;
    public GameObject personPrefab;
    public GameObject placePrefab;
    private List<PlaceBehavior> homes = new List<PlaceBehavior>();
    private List<PlaceBehavior> clinics = new List<PlaceBehavior>();
    private List<PlaceBehavior> offices = new List<PlaceBehavior>();
    public List<GameObject> persons = new List<GameObject>();
    // ***********************

    // VARIABLES -> building process - related
    GameObject placePreviewingObj = null;
    Camera mainCamera;
    private bool isBuidable = true;
    // ***********************

    async void Start()
    {
        // generate place and people
        for (int i = 0; i < clinic_debug.Count; i++)
        {
            GameObject newPlace = Instantiate(placePrefab);
            PlaceBehavior newBehavior = newPlace.GetComponent<PlaceBehavior>();
            newBehavior.init(
                gridValuesAttacher
                ,
                clinic_debug[i]
                );
            clinics.Add(newBehavior);
        }

        for (int i = 0; i < offices_debug.Count; i++)
        {
            GameObject newPlace = Instantiate(placePrefab);
            PlaceBehavior newBehavior = newPlace.GetComponent<PlaceBehavior>();
            newBehavior.init(
                gridValuesAttacher,
                offices_debug[i]
                );
            offices.Add(newBehavior);
        }
        for (int j = 0; j < homes_debug.Count; j++)
        {
            GameObject newPlace = Instantiate(placePrefab);
            PlaceBehavior newBehavior = newPlace.GetComponent<PlaceBehavior>();
            newBehavior.init(
                gridValuesAttacher
                ,
                homes_debug[j]
                );
            homes.Add(newBehavior);
        }

        // after we've set the initial places
        // let's calculate the clarance for the first time 

        await CalculateClarance();

       // initialize all the citizens after we set all the places

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
                        gridValuesAttacher,
                        timeManager,
                        personInfectionManager,
                        newPerson,
                        null,
                        revenueManager,
                        clinics
                        );
                    persons.Add(newPerson);
                }
            }
        }
        GameObject newPerson_ = Instantiate(personPrefab);
        newPerson_.GetComponent<PersonBehavior>().init(
            homes[0],
            offices[0],
            gridValuesAttacher,
            timeManager,
            personInfectionManager,
            newPerson_,
            new Infection(),
            revenueManager,
            clinics
            );
        persons.Add(newPerson_);
        personInfectionManager.someOneJustGotInfected(newPerson_);


        mainCamera = Camera.main;

        //Event subscribetion -> building related

        BuidableBehavior.OnBuildingPreviewStart += (object sender,
        BuidableBehavior.OnBuildingPreviewStartEvnentArg eventArgs) =>
        {
            GeneratePlacePreview(eventArgs.placeType);
            actionCancelRegister.SetActionInstance(PanelType.Build);
        };

        ActionCancelRegister.OnBuildCancel += (sender, e) =>
        {
            DestroyPlacePreview();
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
            Debug.LogError("wtf");
            Destroy(placePreviewingObj);
        }
        placePreviewingObj = Instantiate(placePrefab);
        placePreviewingObj.GetComponent<PlaceBehavior>().shallowInit(placeType);
    }
    public void DestroyPlacePreview()
    {
        Assert.IsTrue(placePreviewingObj != null );
        Destroy(placePreviewingObj);
        //placePreviewingObj = Instantiate(placePrefab);

    }
    public void ConfirmPlacePreview()
    {
        Assert.IsTrue(placePreviewingObj != null);
    }
    // ***********************

    async Task CalculateClarance()
    {
        PlaceClaranceNode[,] gridValue = gridValuesAttacher.placeClaranceGVC.GetGridArray();
        List<Task> claranceCalcuTasks = new List<Task>();
        for (int i = 0; i < gridValuesAttacher.placeClaranceGVC.width; i++)
        {
            for (int j = 0; j < gridValuesAttacher.placeClaranceGVC.height; j++)
            {
                claranceCalcuTasks.Add(gridValue[i, j].CalculateClarance());
            }
        }
        await Task.WhenAll(claranceCalcuTasks);
    }

    async void Update()
    {
        if (placePreviewingObj != null)
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

            Vector3 mouseGridLocalPosition = transform.InverseTransformPoint(mouseWorldPosition);
            mouseGridLocalPosition.z = 0;
            Vector3 mouseCellPosition = GridValuesContainer<PathFindingNode>.localToCell(mouseGridLocalPosition);
            placePreviewingObj.transform.position = mouseCellPosition;
            // Scan for buidability


            int mapWidth = gridValuesAttacher.placeClaranceGVC.width;
            int mapHeight = gridValuesAttacher.placeClaranceGVC.height;

            PlaceBehavior previewPlaceBehavior = placePreviewingObj.GetComponent<PlaceBehavior>();

            int x_size = (int)previewPlaceBehavior.size.x;
            int y_size = (int)previewPlaceBehavior.size.y;
            int x_origin = (int)mouseCellPosition.x;
            int y_origin = (int)mouseCellPosition.y;

            bool isMinEdge = x_origin - 1 < 0 || y_origin - 1 < 0;
            bool isMaxEdge = x_origin + x_size >= mapWidth || y_origin + y_size >= mapHeight;

            //Debug.ClearDeveloperConsole();
            //Debug.Log(y_origin.ToString() + "," + y_size.ToString() + "," + mapHeight.ToString());
            isBuidable = true;
            if (isMaxEdge || isMinEdge)
            {
                isBuidable = false;
            }
            else
            {
                for (int xi = (int)mouseCellPosition.x - 1; xi < (int)mouseCellPosition.x + x_size + 1; xi++)
                {
                    PlaceClaranceNode placeClaranceNode = gridValuesAttacher.placeClaranceGVC.GetGridObj(new Vector3(xi, y_origin));
                    (int n, int s) = placeClaranceNode.GetNorthSouth();
                    if (!(s >= 1 && n >= y_size)) { isBuidable = false; break; }
                }

                for (int yi = (int)mouseCellPosition.y - 1; yi < (int)mouseCellPosition.y + y_size + 1; yi++)
                {
                    PlaceClaranceNode placeClaranceNode = gridValuesAttacher.placeClaranceGVC.GetGridObj(new Vector3(x_origin, yi));
                    (int w, int e) = placeClaranceNode.GetWestEast();
                    if (!(w >= 1 && e >= x_size)) { isBuidable = false; break; }
                }
            }
            placePreviewingObj.GetComponentInChildren<SpriteRenderer>().color = isBuidable ? buidableColor : notBuidableColor;

            if (Input.GetMouseButton(0) && isBuidable)
            {
                


                // fully init £¡
                previewPlaceBehavior.init(
                gridValuesAttacher,
                new PlacePrototype(mouseCellPosition, previewPlaceBehavior.placeType,
                previewPlaceBehavior.size)
                );
                List<PlaceBehavior> placeListToBeUpdated;
                switch (previewPlaceBehavior.placeType)
                {
                    case PlaceType.Clinic: { 
                        placeListToBeUpdated = clinics; 
                        break;}
                    default: { 
                        placeListToBeUpdated = clinics;
                        break;}
                }
               


                clinics.Add(previewPlaceBehavior);
                // invoke an announcement
                OnBuildConfirm?.Invoke(this, new OnBuildConfrimEventArg
                {
                    updatedPlaceBehaviorList = placeListToBeUpdated,
                    //updateType = previewPlaceBehavior.placeType,
                    newPlaceBehavior = previewPlaceBehavior

                });

                // recalculate clarance 
                await CalculateClarance();

                //detach !
                placePreviewingObj = null;
            }
        }
    }
}
