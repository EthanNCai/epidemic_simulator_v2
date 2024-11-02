using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class CamaraBehavior : MonoBehaviour
{ 
   
    private const int MIN_ZOOM_IN = 2;
    private const int MAX_ZOOM_OUT = 9;
    private const int MOUSE_SPEED = 100;
    // Start is called before the first frame update
    public GridValuesAttachedBehavior gridValuesAttachedBehavior;

    //private Vector3 offset; // 用于存储拖拽时的偏移量
    //private bool isDragging = false; // 标记是否正在拖拽
    void Start()
    {
        GetComponent<Camera>().orthographicSize = 8;
        transform.position = 
        gridValuesAttachedBehavior.GetWorldGridCenter();
        
    }

    void Update()
    {

        // handel zoom-in and zoom-out
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        float expectedCamSize = GetComponent<Camera>().orthographicSize + wheel * -4000 * Time.deltaTime;

        GetComponent<Camera>().orthographicSize = math.max(MIN_ZOOM_IN, math.min(expectedCamSize, MAX_ZOOM_OUT));

    }



}
