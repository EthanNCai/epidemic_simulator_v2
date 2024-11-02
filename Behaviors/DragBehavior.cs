using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IDragHandler

{
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
        //we need to call the camera controller method to constrain the camera not out of the game field bound.
        p1 = Camera.main.transform.position - camera_right * Input.GetAxisRaw("Mouse X") * dragSpeed * Time.timeScale
            - (camera_up + camera_forward).normalized * Input.GetAxisRaw("Mouse Y") * dragSpeed * Time.timeScale;
        Camera.main.transform.position = p1;
    }
}
