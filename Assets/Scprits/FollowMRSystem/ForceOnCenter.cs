using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSpace;

/// <summary>
/// 让绑定物体Canvas跟随零平面
/// </summary>
public class ForceOnCenter : MonoBehaviour
{

    public Transform UITransform;

    void Start()
    {
        if (UITransform == null)
            UITransform = transform;

        var screenWidth = Vector3.Distance(FCore.screenPointLeftTop, FCore.screenPointRightTop);
        var screenHight = Vector3.Distance(FCore.screenPointRightTop, FCore.screenPointRightBotton);
        var scale = screenHight / 1080;
        UITransform.localScale = new Vector3(scale, scale, 1);
    }

    void Update()
    {
        Start();
        if (UITransform)
        {
            UITransform.position = FCore.screenCentre;
            UITransform.rotation = FCore.screenRotation;
        }
    }

    private void OnApplicationQuit()
    {
        FCore.SetScreen2D();
    }
}
