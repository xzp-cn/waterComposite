using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace liu
{
    /// <summary>
    /// 录屏按键状态的管理控制
    /// </summary>
    public class RecUIBtnStateManageer : MonoBehaviour
    {

        /// <summary>
        /// 开始录屏按钮、停止录屏按钮
        /// </summary>
        public GameObject startRecBtn, stopRecBtn;

        RecordOperate recordOperate;
        private void Start()
        {
            recordOperate = FindObjectOfType<RecordOperate>();
        }

        private void IsPressStartBtnHandler(bool state)
        {
            IsPressStartBtn(state);
        }

        /// <summary>
        /// 按下开始录像按钮
        /// </summary>
        public void PressStartRecBtn()
        {
            if (recordOperate.StartRec())
            {
                IsPressStartBtn(true);
            }
        }


        /// <summary>
        /// 按下停止录像按钮
        /// </summary>
        public void PressStopRecBtn()
        {
            IsPressStartBtn(false);
            recordOperate.StopRec();
            Debug.LogError("按下停止按钮");
        }

        public void IsPressStartBtn(bool isPressStart)
        {
            stopRecBtn.SetActive(isPressStart);
            startRecBtn.SetActive(!isPressStart);
        }

    }
}