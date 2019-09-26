﻿using FSpace;
using UnityEngine;
/// <summary>
/// 绘制一条从笔尖发出的射线
/// </summary>
public class PenRay : MonoBehaviour
{
    /// <summary>
    /// 射线射到UI上
    /// </summary>
    public bool hitUI_bool = false;


    //UnityEngine.EventSystems.
    public GameObject hitGameObject;

    /// <summary>
    /// 射到UI上空间的位置
    /// </summary>
    public Vector3 hitUIWorldPos;


    private LineRenderer _lineRenderer;


    /// <summary>
    /// 射线的最长长度
    /// </summary>
    public float rayLength;


    /// <summary>
    /// 笔尖的模型
    /// </summary>
    private GameObject trackHandle;
    /// <summary>
    /// 点击显示文字
    /// </summary>
    //private GameObject TextGo = null;


    private void Awake()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Unlit/Texture"));
        Debug.Log(gameObject.name);

    }
    private void Start()
    {
        GameObject LoadtrackHandle = Resources.Load("Prefabs/trackHandle") as GameObject;
        trackHandle = Instantiate(LoadtrackHandle) as GameObject;
        trackHandle.name = "trackHandle";
        trackHandle.transform.parent = transform;
        GameObject.DontDestroyOnLoad(gameObject);
    }


    private void Update()
    {
        rayLength = 5 * FCore.ViewerScale;


        _lineRenderer.SetPosition(0, FCore.penPosition);
        if (FCore.isDraging)//如果当前正在拖拽状态
        {
            //使用记录的拖拽距离来设置射线长
            _lineRenderer.SetPosition(1, FCore.penPosition + FCore.lastDragDistance * FCore.penDirection.normalized);
        }
        else//如果不在拖拽状态
        {
            if (hitUI_bool)
            {
                _lineRenderer.SetPosition(1, hitUIWorldPos);
                //Debug.Log("-----------");
            }
            else
            {
                RaycastHit raycastHit;
                int layer = LayerMask.NameToLayer("Default");
                if (Physics.Raycast(FCore.penRay, out raycastHit, rayLength))//使用设定的射线长度来做射线检测
                {
                    _lineRenderer.SetPosition(1, raycastHit.point);
                }
                else
                {
                    _lineRenderer.SetPosition(1, FCore.penPosition + (rayLength * FCore.penDirection.normalized));
                }
            }
        }
        _lineRenderer.startWidth = 0.0005f * FCore.ViewerScale;
        _lineRenderer.endWidth = 0.0005f * FCore.ViewerScale;


        //笔尖的方向
        trackHandle.transform.position = _lineRenderer.GetPosition(1);
        trackHandle.transform.LookAt(_lineRenderer.GetPosition(0));
    }


    private void OnDestroy()
    {
        Destroy(_lineRenderer.material);
    }
}
