using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class SVRPointerEventData : PointerEventData
{
    public SVRPointerEventData(EventSystem eventSystem)
            : base(eventSystem)
    {
    }

    public Ray worldSpaceRay;
    public Vector2 swipeStart;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<b>Position</b>: " + position);
        sb.AppendLine("<b>delta</b>: " + delta);
        sb.AppendLine("<b>eligibleForClick</b>: " + eligibleForClick);
        sb.AppendLine("<b>pointerEnter</b>: " + pointerEnter);
        sb.AppendLine("<b>pointerPress</b>: " + pointerPress);
        sb.AppendLine("<b>lastPointerPress</b>: " + lastPress);
        sb.AppendLine("<b>pointerDrag</b>: " + pointerDrag);
        sb.AppendLine("<b>worldSpaceRay</b>: " + worldSpaceRay);
        sb.AppendLine("<b>swipeStart</b>: " + swipeStart);
        sb.AppendLine("<b>Use Drag Threshold</b>: " + useDragThreshold);
        return sb.ToString();
    }
}

/// <summary>
/// Static helpers for OVRPointerEventData.
/// </summary>
public static class PointerEventDataExtension
{
    public static bool IsVRPointer(this PointerEventData pointerEventData)
    {
        return (pointerEventData is SVRPointerEventData);
    }

    public static Ray GetRay(this PointerEventData pointerEventData)
    {
        SVRPointerEventData vrPointerEventData = pointerEventData as SVRPointerEventData;
        Assert.IsNotNull(vrPointerEventData);

        return vrPointerEventData.worldSpaceRay;
    }

    public static Vector2 GetSwipeStart(this PointerEventData pointerEventData)
    {
        SVRPointerEventData vrPointerEventData = pointerEventData as SVRPointerEventData;
        Assert.IsNotNull(vrPointerEventData);

        return vrPointerEventData.swipeStart;
    }

    public static void SetSwipeStart(this PointerEventData pointerEventData, Vector2 start)
    {
        SVRPointerEventData vrPointerEventData = pointerEventData as SVRPointerEventData;
        Assert.IsNotNull(vrPointerEventData);

        vrPointerEventData.swipeStart = start;
    }
}
