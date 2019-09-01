using huang.module.ui.settingui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using huang.common.screen;
using CertificateVerify;
using liu;

/// <summary>
/// 处理gEvent信息
/// </summary>
public class ManagergEventInfo : MonoBehaviour
{

    void Start()
    {
        ManagerProjectWindow.CloseProjectionAction = CloseProjectionHandle;
    }



    private void CloseProjectionHandle()
    {
        if (uiSettingObj.activeSelf)
        {
            uISetting.CloseProjection();
        }
        else
        {
            uiSettingObj.transform.localScale = Vector3.zero;
            uiSettingObj.SetActive(true);
            uISetting.CloseProjection();
            Invoke("ActionCloseState", 4f);
        }
        //Debug.Log("ManagergEventInfo.CloseProjectionHandle()  Diao yong111111111");
    }

    void ActionCloseState()
    {
        Debug.Log("ManagergEventInfo.ActionCloseState()  Diao yong");
        uiSettingObj.SetActive(false);
        uiSettingObj.transform.localScale = Vector3.one;
    }

    UISetting uISetting;
    GameObject uiSettingObj;
    /// <summary>
    /// 拿到UISetting信息
    /// </summary>
    public void UISettingHandle()
    {
        if (uISetting == null)
        {
            uISetting = FindObjectOfType<UISetting>();
            uiSettingObj = uISetting.gameObject;
        }
    }
}
