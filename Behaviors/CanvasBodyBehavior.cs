using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CanvasBodyBehavior : MonoBehaviour
{
    public const float DEFAULT_CAMERA_SIZE = 8;
    private Camera mainCamera; 
    private const float OFFSETX = 4f;
    private const float OFFSETY = 2f;
    static private Vector3 offsetTL = new Vector3(-OFFSETX, OFFSETY);
    static private Vector3 offsetTR = new Vector3(OFFSETX, OFFSETY);
    static private Vector3 offsetBL = new Vector3(-OFFSETX, -OFFSETY);
    static private Vector3 offsetBR = new Vector3(OFFSETX, -OFFSETY);
    static private Vector3[] placementCandidates =
        new Vector3[] { offsetTR, offsetTL, offsetBL, offsetBR };
    enum DirectionEnum
    {
        BR,
        BL,
        TR,
        TL,
        DEFAULT
    }
    DirectionEnum directionEnum = DirectionEnum.DEFAULT;
    public RectTransform targetRectTransform; 
    public Vector3 targetPosition;
    public float duration = 0.2f; 
    public bool moving = false;

    private void Start()
    {
        targetRectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        Vector3 parentPosition = transform.parent.position;
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(parentPosition);
        DetectPosition(viewportPosition,false);
        //targetRectTransform.localPosition = offsetTR;
    }

    void Update()
    {
        Vector3 parentPosition = transform.parent.position;
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(parentPosition);

        float scaleFactor = math.sqrt( mainCamera.orthographicSize / DEFAULT_CAMERA_SIZE);
        targetRectTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        DetectPosition(viewportPosition,true);
    }

    void DetectPosition(Vector3 viewportPosition,bool animated)
    {
        bool isLeft = viewportPosition.x < 0.5f;
        bool isRight = viewportPosition.x >= 0.5f;
        bool isTop = viewportPosition.y > 0.5f;
        bool isBottom = viewportPosition.y <= 0.5f;
        bool somethingChanged = false;
        if (isLeft && isTop)
        {
            if (directionEnum != DirectionEnum.BR)
            {
                somethingChanged = true;
            }
            targetPosition = offsetBR;
            directionEnum = DirectionEnum.BR;
        }
        else if (isRight && isTop)
        {
            if (directionEnum != DirectionEnum.BL)
            {
                somethingChanged = true;
            }
            targetPosition = offsetBL;
            directionEnum = DirectionEnum.BL;
            
        }
        else if (isLeft && isBottom)
        {
            if (directionEnum != DirectionEnum.TR)
            {
                somethingChanged = true;
            }
            targetPosition = offsetTR;
            directionEnum = DirectionEnum.TR;
        }
        else if (isRight && isBottom)
        {
            if (directionEnum != DirectionEnum.TL)
            {
                somethingChanged = true;
            }
            targetPosition = offsetTL;
            directionEnum = DirectionEnum.TL;
        }
        if (somethingChanged )
        {
            if (animated)
            {
                StartCoroutine(MoveToPosition(targetPosition));
            }
            else
            {
                targetRectTransform.localPosition = targetPosition;
            }
        }

    }
    IEnumerator MoveToPosition(Vector3 targetPosition, float duration = 0.5f)
    {
        if (duration <= 0)
        {
            targetRectTransform.localPosition = targetPosition;
            yield break; // 结束协程
        }

        Vector3 startPosition = targetRectTransform.localPosition;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            targetRectTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
            //Debug.Log($"Elapsed Time: {elapsedTime}, Duration: {duration}");
        }

        // 确保最终位置
        targetRectTransform.localPosition = targetPosition;
        moving = false;
    }

}
