using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public GridValuesAttachedBehavior gridValuesAttachedBehavior;
    void Start()
    {
        transform.position = 
        gridValuesAttachedBehavior.GetWorldGridCenter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
