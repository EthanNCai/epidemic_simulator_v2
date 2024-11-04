using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 

public class HoverColorChanger : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, IPointerClickHandler { 

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer; 

    public  Color hoverColor; 
    private Transform canvasTransform;
    private bool isSelected = false;
    private Color originalColor;
    private static Vector3 bufferedHightlightSize;

    void Start()
    {
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        canvasTransform = transform.GetChild(0);
        bufferedHightlightSize = transform.localScale * 2;
         mainCamera = Camera.main;
        canvasTransform.gameObject.SetActive(false);
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) { return; }
        SetHightlighted();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) { return; }
        StopHightlighted();
    }

    private void SetHightlighted()
    {
        spriteRenderer.color = hoverColor;
        spriteRenderer.sortingOrder = 1;
        transform.localScale = bufferedHightlightSize;
        canvasTransform.gameObject.SetActive(true);
    }

    private void StopHightlighted()
    {
        spriteRenderer.color = originalColor;
        spriteRenderer.sortingOrder = 0;
        transform.localScale = bufferedHightlightSize / 2;
        canvasTransform.gameObject.SetActive(false);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSelected)
        {
            SetHightlighted();
            isSelected = true;
        }
        else
        {
            StopHightlighted();
            isSelected = false;
        }


    }

}