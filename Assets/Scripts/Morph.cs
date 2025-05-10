using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Morph : MonoBehaviour
{
    public Rect destination;
    public float duration = 1;
    public float delay = 0;

    public bool useScaledTime = true;

    public bool morphInStart = false;
    public bool morphIntoPosition = false;

    private RectTransform rect;

    public Interpolation interpolation;


    public delegate void OnMorphEnd();
    public OnMorphEnd onMorphEnd;

    private void Awake()
    {
        onMorphEnd = () => { };

        rect = GetComponent<RectTransform>();

        if (!morphIntoPosition) return;

        Vector2 initialScale = rect.sizeDelta;
        Vector2 initialposition = rect.anchoredPosition;

        rect.anchoredPosition = new Vector2(destination.x, destination.y);
        rect.sizeDelta = new Vector2(destination.width, destination.height);


        destination.x = initialposition.x;
        destination.y = initialposition.y;
        destination.width = initialScale.x;
        destination.height = initialScale.y;
    }

    private void Start()
    {
        if (morphInStart)
            BeginMorph();
    }

    public void BeginMorph()
    {
        hasCalledDestroy = true;

        StopAllCoroutines();

        StartCoroutine(MorphTransform());
    }

    private bool hasCalledDestroy = false;

    IEnumerator MorphTransform()
    {
        yield return new WaitForSeconds(delay);

        if (hasCalledDestroy)
        {
            CancelMorphGameobject();
            hasCalledDestroy = false;
        }


        Vector2 initialPosition = rect.anchoredPosition;
        Vector2 size = rect.sizeDelta;

        Vector2 destinationPosition = new Vector2(destination.x, destination.y);
        Vector2 destinationSize = new Vector2(destination.width, destination.height);


        for (float i = 0; i < 1; i += (useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime) / duration)
        {

            rect.anchoredPosition = interpolation.LerpWithInterpolation(i, initialPosition, destinationPosition);
            rect.sizeDelta = interpolation.LerpWithInterpolation(i, size, destinationSize);

            yield return null;
        }

        rect.anchoredPosition = destinationPosition;
        rect.sizeDelta = destinationSize;

        onMorphEnd();
    }


    private Vector2 GetParentPosition(RectTransform rect)
    {
        if (rect == null)
        {
            return Vector2.zero;
        }

        return rect.anchoredPosition + GetParentPosition(rect.parent as RectTransform);
    }

    private Vector2 GetParentWorldPosition(RectTransform rect)
    {
        if (rect == null)
        {
            return Vector2.zero;
        }

        return (Vector2)rect.position + GetParentWorldPosition(rect.parent as RectTransform);
    }

    [Header("Debug")]
    public float pixelsPerUnit = 1;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        RectTransform rect = GetComponent<RectTransform>();

        if (rect.anchorMin == rect.anchorMax)
        {
            Vector2 parentPos = GetParentPosition(rect.parent as RectTransform);

            Vector2 pivot = rect.pivot;

            float width = pixelsPerUnit * destination.width;
            float height = pixelsPerUnit * destination.height;

            Vector2 objectPos = new Vector2(

                parentPos.x + (destination.x - destination.width * pivot.x) / pixelsPerUnit,
                parentPos.y + (destination.y - destination.height * pivot.y) / pixelsPerUnit

                );


            Vector2 objectBottomLeft = objectPos;
            Vector2 objectBottomRight = objectPos + new Vector2(width, 0);
            Vector2 objectTopRight = objectPos + new Vector2(width, height);
            Vector2 objectTopLeft = objectPos + new Vector2(0, height);

            Vector3[] points = new Vector3[]
            {
                objectBottomLeft,
                objectBottomRight,

                objectBottomRight,
                objectTopRight,

                objectTopRight,
                objectTopLeft,

                objectTopLeft,
                objectBottomLeft,


                objectBottomLeft,
                parentPos +  rect.anchoredPosition - rect.sizeDelta * new Vector2(pivot.x,pivot.y),

                objectBottomRight,
                parentPos +rect.anchoredPosition - rect.sizeDelta * new Vector2(-pivot.x,pivot.y),

                objectTopRight,
                parentPos +rect.anchoredPosition - rect.sizeDelta * new Vector2(-pivot.x,-pivot.y),

                objectTopLeft,
                parentPos +rect.anchoredPosition - rect.sizeDelta * new Vector2(pivot.x,-pivot.y)
            };


            Gizmos.DrawLineList(points);
        }
    }

    private void Reset()
    {
        RectTransform rect = GetComponent<RectTransform>();
        destination.x = rect.anchoredPosition.x;
        destination.y = rect.anchoredPosition.y;
        destination.width = rect.sizeDelta.x;
        destination.height = rect.sizeDelta.y;
    }


    public void CancelMorphGameobject()
    {
        gameObject.SendMessage(nameof(CancelMorph));
    }

    public void CancelMorph()
    {
        if (hasCalledDestroy)
        {
            hasCalledDestroy = false;
            return;
        }

        StopAllCoroutines();
    }

}
