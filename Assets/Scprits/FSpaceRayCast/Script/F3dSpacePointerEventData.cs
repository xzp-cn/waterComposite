﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEngine.EventSystems
{
    public class F3dSpacePointerEventData : PointerEventData
    {
        public F3dSpacePointerEventData(EventSystem eventSystem) : base(eventSystem)
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
    /// Static helpers for F3dSpacePointerEventData.
    /// </summary>
    public static class PointerEventDataExtension
    {

        public static bool IsF3dSpacePointer(this PointerEventData pointerEventData)
        {
            return (pointerEventData is F3dSpacePointerEventData);
        }
        public static Ray GetRay(this PointerEventData pointerEventData)
        {
            F3dSpacePointerEventData vrPointerEventData = pointerEventData as F3dSpacePointerEventData;
            Assert.IsNotNull(vrPointerEventData);

            return vrPointerEventData.worldSpaceRay;
        }
        public static Vector2 GetSwipeStart(this PointerEventData pointerEventData)
        {
            F3dSpacePointerEventData vrPointerEventData = pointerEventData as F3dSpacePointerEventData;
            Assert.IsNotNull(vrPointerEventData);

            return vrPointerEventData.swipeStart;
        }
        public static void SetSwipeStart(this PointerEventData pointerEventData, Vector2 start)
        {
            F3dSpacePointerEventData vrPointerEventData = pointerEventData as F3dSpacePointerEventData;
            Assert.IsNotNull(vrPointerEventData);

            vrPointerEventData.swipeStart = start;
        }
    }
}
