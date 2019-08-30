using FSpace;
using huang.common.recordscreentool;
using liu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 程序窗口交互
/// </summary>
public class WindowsOperate : MonoBehaviour
{


    private void OnEnable()
    {
        FCore.SetScreen3DSelf();
    }
    /// <summary>
    /// 程序退出
    /// </summary>
    public void ProgramApplicationQuit()
    {
        var recordOperate = FindObjectOfType<RecordOperate>();
        if (recordOperate)
        {
            if(recordOperate.startFlag) RecordScreenTool.Instance.StopRecoreScreen();
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
