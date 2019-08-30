using liu;
using UnityEngine;


/// <summary>
/// 标签面板正对摄像机
/// </summary>
public class PanelControl : MonoBehaviour
{

    private Transform LookPos;
    public Transform followStartPos;
    public Transform followEndPos;

    Transform selfStartPos, selfEndPos;

    Transform PanelCanvas;
    LineRenderer RenderLine;

    void Start()
    {
        if (GameObject.Find("Camera3D") != null)
            LookPos = GameObject.Find("Camera3D").gameObject.transform;
        //LookPos = transform;
        selfStartPos = transform.Find("StartPos");
        selfEndPos = transform.Find("End");

        //隱藏StartPos和End 上的“Cube”，Cube是用來觀看標簽的起始位置
        if (null == followStartPos)
            FindObjExitAndHideThis(followStartPos, "Cube");
        if (null == followEndPos)
            FindObjExitAndHideThis(followEndPos, "Cube");

        RenderLine = GetComponent<LineRenderer>();
        PanelCanvas = transform.GetComponentInChildren<Canvas>().transform;
        MeshRenderer[] meshs = transform.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshs.Length; i++)
        {
            meshs[i].enabled = false;
        }
    }

    /// <summary>
    /// 找傳過來的Transform下的物體
    /// </summary>
    /// <param name="hideChilTrans">當前要找的父物體</param>
    /// <param name="chilName">chil的name</param>
    void FindObjExitAndHideThis(Transform hideChilTrans, string chilName)
    {
        Transform findObj = hideChilTrans.Find(chilName);
        if (findObj) { findObj.gameObject.SetActive(false); }
    }

    void Update()
    {
        if (Monitor23DMode.instance == null)
        {
            LookPos = Camera.main.transform;
        }
        else
        {
            if (Monitor23DMode.instance.is3D)
            {
                LookPos = Monitor23DMode.instance.CameraLeft.transform;
            }
            else
            {
                LookPos = Monitor23DMode.instance.camera2D.transform;
            }
        }
        //if (LookPos != null && LookPos.gameObject.activeInHierarchy && PanelCanvas)
        //    PanelCanvas.LookAt(LookPos);

        if (RenderLine != null)
        {
            RenderLine.SetPosition(0, followStartPos.position);
            selfStartPos.position = followStartPos.position;

            RenderLine.SetPosition(1, followEndPos.position);
            selfEndPos.position = followEndPos.position;
        }
    }
}
