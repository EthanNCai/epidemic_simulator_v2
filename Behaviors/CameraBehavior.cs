using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CameraBehavior : MonoBehaviour
{ 
   
    private const int MIN_ZOOM_IN = 2;
    private const int MAX_ZOOM_OUT = 9;
    private const int MOUSE_SPEED = 100;
    private const float MARGIN = 1;
    private const float DEFAULT_CAMERA_SIZE = 8;
    private Vector3 bottomLeft;
    private Vector3 topRight;
    


    // Start is called before the first frame update
    public GridValuesAttachedBehavior gridValuesAttachedBehavior;

    //private Vector3 offset; // 用于存储拖拽时的偏移量
    //private bool isDragging = false; // 标记是否正在拖拽
    void Start()
    {
        GetComponent<Camera>().orthographicSize = DEFAULT_CAMERA_SIZE;
        transform.position = 
        gridValuesAttachedBehavior.GetWorldGridCenter();
        topRight = gridValuesAttachedBehavior.GetWorldTopRightCorner();
        bottomLeft = gridValuesAttachedBehavior.GetWorldBottomLeftCorner();
    }

    void Update()
    {

        // handel zoom-in and zoom-out
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        float expectedCamSize = GetComponent<Camera>().orthographicSize + wheel  * -20;

        GetComponent<Camera>().orthographicSize =

            math.clamp(expectedCamSize, MIN_ZOOM_IN, MAX_ZOOM_OUT);

    }

    public float GetDragSensitivityRatio()
    {
        return GetComponent<Camera>().orthographicSize / DEFAULT_CAMERA_SIZE;
    }

    public void UpdateCameraPositionAttempt(Vector3 attemptPosition)
    {
        Vector3 ruledPosition = new Vector3(
            math.clamp(attemptPosition.x, 
            bottomLeft.x - MARGIN, topRight.x + MARGIN),
            math.clamp(attemptPosition.y, 
            bottomLeft.y - MARGIN, topRight.y + MARGIN),
            -10
            );

        Camera.main.transform.position = ruledPosition;
    }



}
