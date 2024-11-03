using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // 用于事件系统

public class HoverColorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SpriteRenderer spriteRenderer; // SpriteRenderer组件
    //public Color defaultColor; // 默认颜色
    public Color hoverColor; // 悬浮时的颜色

    private Color originalColor; // 原始颜色，用于恢复

    void Start()
    {
        // 初始化时保存原始颜色
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        Debug.Log("hover");
    }

    // 当鼠标悬浮时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 改变颜色
        spriteRenderer.color = hoverColor;
        spriteRenderer.sortingOrder = 1;
        transform.localScale *= 2;
    }

    // 当鼠标离开时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        // 恢复原始颜色
        spriteRenderer.color = originalColor;
        spriteRenderer.sortingOrder = 0;
        transform.localScale /= 2;
    }
}