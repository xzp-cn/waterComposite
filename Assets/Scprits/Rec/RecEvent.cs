using huang.common.recordscreentool;
using huang.common.screen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace liu
{
    /// <summary>
    /// 录屏事件
    /// </summary>
    public class RecEvent : MonoBehaviour
    {
        /// <summary>
        /// 停止录制按钮
        /// </summary>
        Button stopRecBtn;

        /// <summary>
        /// 录屏按钮的提示问题
        /// </summary>
        GameObject recTip;

        RecordOperate recordOperate;
        RecUIBtnStateManageer recUIBtnStateManageer;

        //ScreenControlObj _screenControlObj;
        //ScreenControlObj screenControlObj
        //{
        //    get { if (_screenControlObj == null) _screenControlObj = FindObjectOfType<ScreenControlObj>(); return _screenControlObj; }
        //}
        private void Start()
        {
            //隐藏RecUI的UI
            transform.GetChild(0).gameObject.SetActive(false);

            recordOperate = FindObjectOfType<RecordOperate>();
            recUIBtnStateManageer = FindObjectOfType<RecUIBtnStateManageer>();

            stopRecBtn = transform.Find("BackGround/Button").GetComponent<Button>();
            recTip = stopRecBtn.transform.Find("Tip").gameObject;

            recTip.SetActive(false);

            stopRecBtn.onClick.AddListener(() => 
            {
                RecordScreenTool.Instance.StopRecoreScreen();
                //screenControlObj.CloseRecWin();
                ShowRecUI(false);
                recordOperate.startFlag = false;
            });
        }

        /// <summary>
        /// 显示录屏显示时间的UI
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowRecUI(bool isShow)
        {
            //transform.GetChild(0).gameObject = RecUI/BackGround
            transform.GetChild(0).gameObject.SetActive(isShow);
            recUIBtnStateManageer.IsPressStartBtn(false);
        }
    }
}