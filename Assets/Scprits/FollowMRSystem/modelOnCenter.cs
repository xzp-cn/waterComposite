using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSpace;

/// <summary>
/// 让绑定的物体
/// </summary>
public class modelOnCenter : MonoBehaviour
{

    public Transform Models;

    void Start()
    {
        if (Models == null)
            Models = transform;
    }

    void Update()
    {
        if (Models)
        {
            Models.position = FCore.screenCentre;
            Models.rotation = FCore.screenRotation;

        }
    }
}
