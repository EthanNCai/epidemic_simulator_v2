using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // �����¼�ϵͳ

public class HoverColorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SpriteRenderer spriteRenderer; // SpriteRenderer���
    //public Color defaultColor; // Ĭ����ɫ
    public Color hoverColor; // ����ʱ����ɫ

    private Color originalColor; // ԭʼ��ɫ�����ڻָ�

    void Start()
    {
        // ��ʼ��ʱ����ԭʼ��ɫ
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        Debug.Log("hover");
    }

    // ���������ʱ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        // �ı���ɫ
        spriteRenderer.color = hoverColor;
        spriteRenderer.sortingOrder = 1;
        transform.localScale *= 2;
    }

    // ������뿪ʱ����
    public void OnPointerExit(PointerEventData eventData)
    {
        // �ָ�ԭʼ��ɫ
        spriteRenderer.color = originalColor;
        spriteRenderer.sortingOrder = 0;
        transform.localScale /= 2;
    }
}