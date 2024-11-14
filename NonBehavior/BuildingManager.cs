using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject buidableUIPrefabs;
    public GameObject placePrefab;
    void Start()
    {
        //transform.GetChild(0);
        // generate buidables.
        GameObject temp = Instantiate(buidableUIPrefabs,transform);
        temp.GetComponent<BuidableBehavior>().init(PlaceType.Clinic);
        //GameObject temp2 = Instantiate(buidableUIPrefabs, transform);
        //temp2.GetComponent<BuidableBehavior>().init(PlaceType.Hospital);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
}
