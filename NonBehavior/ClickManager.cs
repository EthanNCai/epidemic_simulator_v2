using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public GridValuesAttachedBehavior gridValuesAttachedBehavior;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            Vector3 mouseLocalPosition = transform.TransformPoint(mouseWorldPosition);
            gridValuesAttachedBehavior.pathFindingGVC.GetGridObj(mouseLocalPosition).ToggleWalkable();
        }

    }
}
