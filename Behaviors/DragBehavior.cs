using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IDragHandler
{
    public CameraBehavior cameraBehavior;
    public float dragSpeed;
    private Vector3 p1;
    private Vector3 camera_right;
    private Vector3 camera_up;
    private Vector3 camera_forward;

    public void Awake()
    {
        camera_right = Camera.main.transform.right;
        camera_up = Camera.main.transform.up;
        camera_forward = Camera.main.transform.forward;
    }
    public void OnDrag(PointerEventData eventData)
    {
        //if (eventData.pointerEnter.layer == LayerMask.NameToLayer("HiddenUI"))
        //{
            float dragSpeedRatio = cameraBehavior.GetDragSensitivityRatio();
            p1 = Camera.main.transform.position - camera_right * Input.GetAxisRaw("Mouse X") * dragSpeed * dragSpeedRatio
                - (camera_up + camera_forward).normalized * Input.GetAxisRaw("Mouse Y") * dragSpeed * dragSpeedRatio;

            cameraBehavior.UpdateCameraPositionAttempt(p1);
        //}
    }
}
