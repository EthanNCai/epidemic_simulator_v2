using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PersonSelectionManager : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, IPointerClickHandler {

    public ActionCancelRegister actionCancelRegister;
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer; 
    private PersonBehavior personBehavior;
    public  Color hoverColor; 
    private Transform canvasTransform;
    private bool isSelected = false;
    private bool isHover = false;
    private Color originalColor;
    private static Vector3 bufferedHightlightSize;
    Transform imgTransform;
    TextMeshProUGUI IDTextTMP;
    TextMeshProUGUI InfectionStateTMP;
    TextMeshProUGUI SocialStateTMP;


    void Start()
    {
        actionCancelRegister = GameObject.FindFirstObjectByType<ActionCancelRegister>();
        personBehavior = GetComponent<PersonBehavior>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        canvasTransform = transform.GetChild(0);
        bufferedHightlightSize = transform.localScale * 2;
         mainCamera = Camera.main;
        canvasTransform.gameObject.SetActive(false);

        // find all the TMPROS
        //canvasTransform = transform.GetChild(0);
        imgTransform = canvasTransform.GetChild(0);

        TextMeshProUGUI[] textMeshs = imgTransform.GetComponentsInChildren<TextMeshProUGUI>();
        //Debug.Log(textMeshs.Length);
        IDTextTMP = textMeshs[0];
        InfectionStateTMP = textMeshs[1];
        SocialStateTMP = textMeshs[2];

        ActionCancelRegister.OnPersonInspectClosing += (sender, e) =>
        {
            Unselect();
        };
    }

    void Update()
    {
        if (isSelected || isHover)
        {
            SendInfosToUI();
        }
    }

    private void SendInfosToUI()
    {
        IDTextTMP.text = personBehavior._name;
        InfectionStateTMP.text = Infection.GetInfectionStatusDescriber(personBehavior.infectionStatus);
        SocialStateTMP.text = PersonBehavior.GetSocialStatusDescriber(personBehavior);
    }

    public void Unselect()
    {
        isSelected = false;
        StopHightlighted();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
        if (isSelected) { return; }
        SendInfosToUI();
        SetHightlighted();
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
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
            actionCancelRegister.SetActionInstance(PanelType.PersonInspect);
        }
        else
        {
            StopHightlighted();
            isSelected = false;
        }
    }
}